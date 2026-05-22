CREATE DATABASE BD_TRANSACCIONES_OMM;

USE BD_TRANSACCIONES_OMM;

CREATE LOGIN USUARIO WITH PASSWORD = 'PASSWORD';
CREATE USER USUARIO FOR LOGIN USUARIO;
ALTER ROLE db_owner ADD MEMBER USUARIO;

CREATE TABLE moneda (
    codigo CHAR(3) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    simbolo NVARCHAR(5)
);

CREATE TABLE cuenta (
    nro_cuenta NVARCHAR(14) PRIMARY KEY,
    tipo CHAR(3) CHECK (TIPO IN ('AHO', 'CTE')),
    moneda CHAR(3),
    nombre NVARCHAR(40),
    saldo DECIMAL(12, 2)
	CONSTRAINT FK_CUENTA_MONEDA FOREIGN KEY (moneda) REFERENCES moneda(codigo),
);

CREATE TABLE movimiento (
	fecha DATETIME2 PRIMARY KEY,
	tipo char(1) CHECK (TIPO IN ('D', 'A')),
	importe decimal(12,2),
	nro_cuenta NVARCHAR(14),
	CONSTRAINT FK_MOVIMEINTO_CUENTA FOREIGN KEY (nro_cuenta) REFERENCES cuenta(nro_cuenta),
);

CREATE TABLE tipo_cambio (
    id INT IDENTITY(1,1) PRIMARY KEY,
    fecha DATE NOT NULL,
    moneda_origen CHAR(3) NOT NULL,
    moneda_destino CHAR(3) NOT NULL,
    tasa DECIMAL(10, 6) NOT NULL,
    CONSTRAINT FK_TIPO_CAMBIO_MONEDA_ORIGEN FOREIGN KEY (moneda_origen) REFERENCES moneda(codigo),
    CONSTRAINT FK_TIPO_CAMBIO_MONEDA_DESTINO FOREIGN KEY (moneda_destino) REFERENCES moneda(codigo)
);

-- procedimientos almacenados
CREATE OR ALTER PROCEDURE SumarizarMovimientosYActualizarCuentas
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Crear una tabla temporal con un índice cluster
    CREATE TABLE #TempSumaMovimientos (
        nro_cuenta VARCHAR(50) PRIMARY KEY CLUSTERED,
        importe DECIMAL(18, 2)
    );
    
    -- Insertar las sumas de importe por número de cuenta en la tabla temporal
    -- Excluyendo los registros donde nro_cuenta es NULL
    INSERT INTO #TempSumaMovimientos (nro_cuenta, importe)
    SELECT nro_cuenta, SUM(Importe) AS Saldo
    FROM Movimiento WITH (NOLOCK)
    WHERE nro_cuenta IS NOT NULL
    GROUP BY nro_cuenta;
    
    -- Actualizar la tabla Cuenta con los saldos calculados
    UPDATE C
    SET Saldo = T.importe
    FROM Cuenta C
    INNER JOIN #TempSumaMovimientos T ON C.nro_cuenta = T.nro_cuenta;
    
    -- Eliminar la tabla temporal
    DROP TABLE #TempSumaMovimientos;
END;


-- procedimiento para realizar transferencia entre cuentas
CREATE OR ALTER PROCEDURE RealizarTransferencia
    @CuentaOrigenID NVARCHAR(14),
    @CuentaDestinoID NVARCHAR(14),
    @Monto DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    DECLARE @MonedaOrigen CHAR(3);
    DECLARE @MonedaDestino CHAR(3);
    DECLARE @TipoCambio DECIMAL(10,6);
    DECLARE @MontoConvertido DECIMAL(18,2);
    DECLARE @FechaMovimientoOrigen DATETIME2 = SYSDATETIME();
    DECLARE @FechaMovimientoDestino DATETIME2 = DATEADD(MILLISECOND, 1, @FechaMovimientoOrigen);
    DECLARE @FechaActual DATE = CAST(@FechaMovimientoOrigen AS DATE);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener las monedas de las cuentas
        SELECT @MonedaOrigen = Moneda FROM Cuenta WHERE CAST(nro_cuenta AS INT) = @CuentaOrigenID;
        SELECT @MonedaDestino = Moneda FROM Cuenta WHERE CAST(nro_cuenta AS INT) = @CuentaDestinoID;

        -- Si las monedas son iguales, no se necesita conversión
        IF @MonedaOrigen = @MonedaDestino
        BEGIN
            SET @MontoConvertido = @Monto;
        END
        ELSE
        BEGIN
            -- Obtener el tipo de cambio más reciente
            SELECT TOP 1 @TipoCambio = tasa
            FROM tipo_cambio
            WHERE moneda_origen = @MonedaOrigen
              AND moneda_destino = @MonedaDestino
              AND fecha <= @FechaActual
            ORDER BY fecha DESC;

            -- Si no se encuentra un tipo de cambio directo, intentar la conversión inversa
            IF @TipoCambio IS NULL
            BEGIN
                SELECT TOP 1 @TipoCambio = 1 / tasa
                FROM tipo_cambio
                WHERE moneda_origen = @MonedaDestino
                  AND moneda_destino = @MonedaOrigen
                  AND fecha <= @FechaActual
                ORDER BY fecha DESC;
            END

            -- Si aún no se encuentra un tipo de cambio, lanzar un error
            IF @TipoCambio IS NULL
            BEGIN
                THROW 50001, 'No se encontró un tipo de cambio válido para la conversión', 1;
            END

            -- Realizar la conversión
            SET @MontoConvertido = @Monto * @TipoCambio;
        END

        -- Registrar el movimiento de débito
        INSERT INTO Movimiento (Fecha, nro_cuenta, Importe, Tipo)
        VALUES (@FechaMovimientoOrigen, @CuentaOrigenID, -@Monto, 'D');

        -- Registrar el movimiento de crédito
        INSERT INTO Movimiento (Fecha, nro_cuenta, Importe, Tipo)
        VALUES (@FechaMovimientoDestino, @CuentaDestinoID, @MontoConvertido, 'A');

        -- Actualizar saldos
        UPDATE Cuenta
        SET Saldo = Saldo - @Monto
        WHERE CAST(nro_cuenta AS INT) = @CuentaOrigenID;

        UPDATE Cuenta
        SET Saldo = Saldo + @MontoConvertido
        WHERE CAST(nro_cuenta AS INT) = @CuentaDestinoID;

        COMMIT TRANSACTION;

        -- Devolver información sobre la transacción
        SELECT 
            @FechaMovimientoOrigen AS Fecha,
            @CuentaOrigenID AS CuentaOrigenID,
            @CuentaDestinoID AS CuentaDestinoID,
            @Monto AS MontoOrigen,
            @MontoConvertido AS MontoDestino,
            @MonedaOrigen AS MonedaOrigen,
            @MonedaDestino AS MonedaDestino,
            @TipoCambio AS TipoCambioUtilizado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
