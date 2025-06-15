CREATE TABLE users (
    userID INT IDENTITY(1,1) PRIMARY KEY, 
    username VARCHAR(50) NOT NULL,
    pass_word VARCHAR(255) NOT NULL, 
    email VARCHAR(100) UNIQUE NOT NULL,
    userRole VARCHAR(20) DEFAULT 'user'
);

CREATE TABLE orders(
    orderID INT IDENTITY(1,1) PRIMARY KEY,
	userID INT NOT NULL,
	orderDate DATE NOT NULL,
	totalAmount NUMERIC(10,2) NOT NULL,
	orderStatus VARCHAR(20) NOT NULL, --pending, completed or cancelled
	FOREIGN KEY (userID) REFERENCES users(userID)
);

ALTER TABLE orders ADD quantity INT DEFAULT 1;

ALTER TABLE orders
ADD CONSTRAINT DF_orders_orderStatus DEFAULT 'Pending' FOR orderStatus;

ALTER TABLE orders
ADD gameID INT NOT NULL;
ALTER TABLE orders
ADD CONSTRAINT FK_orders_gameID FOREIGN KEY (gameID) REFERENCES game(gameID);

CREATE TABLE payment(
   paymentID INT IDENTITY(1,1) PRIMARY KEY,
   orderID INT NOT NULL,
   paymentDate DATE NOT NULL,
   paymentStatus VARCHAR(20) NOT NULL,
   paymentMethod VARCHAR(20) NOT NULL,
   FOREIGN KEY (orderID) REFERENCES orders(orderID)
  );

  ALTER TABLE payment
ADD CONSTRAINT DF_payment_paymentDate DEFAULT GETDATE() FOR paymentDate;

CREATE TABLE game(
    gameID INT IDENTITY(1,1) PRIMARY KEY,
	title VARCHAR(50) NOT NULL,
	gameDescription VARCHAR(255) NOT NULL,
	price NUMERIC(10,2) NOT NULL,
	genre VARCHAR(50)
 );

CREATE TABLE review(
   reviewID INT IDENTITY(1,1) PRIMARY KEY,
   userID INT NOT NULL,
   gameID INT NOT NULL,
   rating INT NOT NULL CHECK (rating >=1 AND rating <=5),
   reviewText VARCHAR(1000),
   dateAdded DATE NOT NULL,
   FOREIGN KEY (gameID) REFERENCES game(gameID),
   FOREIGN KEY (userID) REFERENCES users(userID)
);

ALTER TABLE review
ADD CONSTRAINT DF_review_dateAdded DEFAULT GETDATE() FOR dateAdded;

ALTER TABLE review
DROP CONSTRAINT CK__review__rating__4316F928;

ALTER TABLE review
DROP COLUMN rating;

CREATE TABLE inventory (
    gameID INT NOT NULL,
	stockQuantity INT NOT NULL,
   FOREIGN KEY (gameID) REFERENCES game(gameID)
);