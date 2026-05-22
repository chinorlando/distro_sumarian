USE BD_TRANSACCIONES_OMM_NEW;
GO

CREATE PROCEDURE sp_RealizarTransferencia_OMM
    @CuentaOrigen NVARCHAR(14),
    @CuentaDestino NVARCHAR(14),
    @Monto DECIMAL(12,2) -- Monto en la moneda de la cuenta origen
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Validar existencia de cuentas
        IF NOT EXISTS (SELECT 1 FROM CUENTA WHERE NRO_CUENTA = @CuentaOrigen) OR 
           NOT EXISTS (SELECT 1 FROM CUENTA WHERE NRO_CUENTA = @CuentaDestino)
        BEGIN
            RAISERROR('Una o ambas cuentas no existen.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Obtener monedas y saldos
        DECLARE @MonedaOrigen CHAR(3), @MonedaDestino CHAR(3), @SaldoOrigen DECIMAL(12,2);
        SELECT @MonedaOrigen = MONEDA, @SaldoOrigen = SALDO FROM CUENTA WHERE NRO_CUENTA = @CuentaOrigen;
        SELECT @MonedaDestino = MONEDA FROM CUENTA WHERE NRO_CUENTA = @CuentaDestino;

        -- 3. Validar saldo suficiente
        IF (@SaldoOrigen < @Monto)
        BEGIN
            RAISERROR('Saldo insuficiente en la cuenta de origen.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. Calcular monto destino (con conversión si es necesario)
        DECLARE @MontoDestino DECIMAL(12,2) = @Monto;
        DECLARE @Tasa DECIMAL(12,4) = 1.0;

        IF (@MonedaOrigen <> @MonedaDestino)
        BEGIN
            SELECT TOP 1 @Tasa = Tasa 
            FROM TipoCambio 
            WHERE MonedaOrigen = @MonedaOrigen AND MonedaDestino = @MonedaDestino
            ORDER BY Fecha DESC;

            IF (@Tasa IS NULL)
            BEGIN
                RAISERROR('No se encontró un tipo de cambio configurado para esta conversión.', 16, 1);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            SET @MontoDestino = @Monto * @Tasa;
        END

        -- 5. Ejecutar débitos y créditos
        UPDATE CUENTA SET SALDO = SALDO - @Monto WHERE NRO_CUENTA = @CuentaOrigen;
        UPDATE CUENTA SET SALDO = SALDO + @MontoDestino WHERE NRO_CUENTA = @CuentaDestino;

        -- 6. Registrar movimientos
        DECLARE @FechaActual DATETIME = GETDATE();
        INSERT INTO MOVIMIENTO (NRO_CUENTA, FECHA, TIPO, IMPORTE) VALUES (@CuentaOrigen, @FechaActual, 'D', @Monto);
        INSERT INTO MOVIMIENTO (NRO_CUENTA, FECHA, TIPO, IMPORTE) VALUES (@CuentaDestino, DATEADD(MILLISECOND, 10, @FechaActual), 'A', @MontoDestino);

        COMMIT TRANSACTION;
        SELECT 'Transferencia exitosa' AS Message, @MontoDestino AS MontoRecibido, @Tasa AS TasaAplicada;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@Msg, 16, 1);
    END CATCH
END
GO
