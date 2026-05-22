USE BD_TRANSACCIONES_OMM_NEW; -- Ajustar al nombre de tu BD
GO

CREATE PROCEDURE sp_ProcesarMovimiento_OMM
    @NroCuenta NVARCHAR(14),
    @Tipo CHAR(1), -- 'A' Abono, 'D' Debito
    @Importe DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Validar existencia de la cuenta
        IF NOT EXISTS (SELECT 1 FROM CUENTA WHERE NRO_CUENTA = @NroCuenta)
        BEGIN
            RAISERROR('La cuenta especificada no existe.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Validar saldo suficiente para retiros
        IF (@Tipo = 'D')
        BEGIN
            DECLARE @SaldoActual DECIMAL(12,2);
            SELECT @SaldoActual = SALDO FROM CUENTA WHERE NRO_CUENTA = @NroCuenta;

            IF (@SaldoActual < @Importe)
            BEGIN
                RAISERROR('Saldo insuficiente para realizar el retiro.', 16, 1);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- 3. Actualizar el saldo de la cuenta
        IF (@Tipo = 'A')
            UPDATE CUENTA SET SALDO = SALDO + @Importe WHERE NRO_CUENTA = @NroCuenta;
        ELSE
            UPDATE CUENTA SET SALDO = SALDO - @Importe WHERE NRO_CUENTA = @NroCuenta;

        -- 4. Registrar el movimiento
        INSERT INTO MOVIMIENTO (NRO_CUENTA, FECHA, TIPO, IMPORTE)
        VALUES (@NroCuenta, GETDATE(), @Tipo, @Importe);

        COMMIT TRANSACTION;
        SELECT 'Operación procesada con éxito' AS Message;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
