
CREATE VIEW vw_ActiveOrders
AS
SELECT 
    o.orderID,
    u.username,
    g.title AS gameTitle,
    o.orderDate,
    o.totalAmount,
    o.orderStatus
FROM orders o
JOIN users u ON o.userID = u.userID
JOIN game g ON o.gameID = g.gameID
WHERE o.orderStatus IN ('Pending', 'Processing');

ALTER VIEW vw_ActiveOrders
AS
SELECT 
    o.orderID,
    u.username,
    g.title AS gameTitle,
    o.orderDate,
    o.totalAmount,
    o.orderStatus
FROM orders o
JOIN users u ON o.userID = u.userID
JOIN game g ON o.gameID = g.gameID
WHERE o.orderStatus = 'Pending';


CREATE NONCLUSTERED INDEX idx_users_username
ON users(username);

CREATE NONCLUSTERED INDEX idx_orders_userID
ON orders(userID);

CREATE NONCLUSTERED INDEX idx_review_gameID
ON review(gameID);
