USE BD_TRANSACCIONES_OMM; -- Asegúrate de que el nombre de la BD sea el correcto
GO

CREATE PROCEDURE sp_RegistrarCuenta_OMM
    @NroCuenta NVARCHAR(14),
    @Tipo CHAR(3),
    @Moneda CHAR(3),
    @Nombre NVARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Validar si la cuenta ya existe
    IF EXISTS (SELECT 1 FROM CUENTA WHERE NRO_CUENTA = @NroCuenta)
    BEGIN
        RAISERROR('El número de cuenta ya se encuentra registrado.', 16, 1);
        RETURN;
    END

    -- 2. Validar si la moneda existe
    IF NOT EXISTS (SELECT 1 FROM MONEDA WHERE Codigo = @Moneda)
    BEGIN
        RAISERROR('El código de moneda no es válido.', 16, 1);
        RETURN;
    END

    -- 3. Validar longitud según tipo (13 para CTE, 14 para AHO)
    IF (@Tipo = 'CTE' AND LEN(@NroCuenta) <> 13)
    BEGIN
        RAISERROR('La cuenta corriente (CTE) debe tener 13 caracteres.', 16, 1);
        RETURN;
    END

    IF (@Tipo = 'AHO' AND LEN(@NroCuenta) <> 14)
    BEGIN
        RAISERROR('La cuenta de ahorros (AHO) debe tener 14 caracteres.', 16, 1);
        RETURN;
    END

    -- 4. Inserción con saldo inicial 0
    INSERT INTO CUENTA (NRO_CUENTA, TIPO, MONEDA, NOMBRE, SALDO)
    VALUES (@NroCuenta, @Tipo, @Moneda, @Nombre, 0.00);

    SELECT 'Cuenta registrada exitosamente' AS Message;
END
GO
