CREATE TRIGGER updateInventory
ON orders
AFTER INSERT
AS
BEGIN
    -- Reduce stock quantity for each inserted order
    UPDATE i
    SET i.stockQuantity = i.stockQuantity - 1
    FROM inventory i
    INNER JOIN INSERTED ins ON i.gameID = ins.gameID;
END;
--trigger that updates inventory 
CREATE OR ALTER TRIGGER updateInventory
ON orders
AFTER INSERT
AS
BEGIN
    UPDATE i
    SET i.stockQuantity = i.stockQuantity - ins.quantity
    FROM inventory i
    INNER JOIN INSERTED ins ON i.gameID = ins.gameID;
END;

CREATE TRIGGER trg_updateOrderStatus
ON payment
AFTER INSERT, UPDATE
AS
BEGIN
    -- Whenever a payment is inserted or updated to 'Completed'
    UPDATE o
    SET o.orderStatus = 'Completed'
    FROM orders o
    INNER JOIN inserted p ON o.orderID = p.orderID
    WHERE p.paymentStatus = 'Completed';
END;

CREATE TRIGGER trg_restockInventory
ON orders
AFTER UPDATE
AS
BEGIN
    UPDATE i
    SET i.stockQuantity = i.stockQuantity + 1
    FROM inventory i
    INNER JOIN inserted ins ON i.gameID = ins.gameID
    INNER JOIN deleted d ON d.orderID = ins.orderID
    WHERE ins.orderStatus = 'Cancelled' AND d.orderStatus <> 'Cancelled';
END;

CREATE OR ALTER TRIGGER trg_restockInventory
ON orders
AFTER UPDATE
AS
BEGIN
    -- Re-add quantity to inventory only when status changes TO 'Cancelled'
    UPDATE i
    SET i.stockQuantity = i.stockQuantity + ins.quantity
    FROM inventory i
    INNER JOIN inserted ins ON i.gameID = ins.gameID
    INNER JOIN deleted d ON d.orderID = ins.orderID
    WHERE ins.orderStatus = 'Cancelled' AND d.orderStatus <> 'Cancelled';
END;
