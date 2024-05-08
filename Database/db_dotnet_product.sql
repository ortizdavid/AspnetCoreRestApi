DROP DATABASE IF EXISTS db_dotnet_product;
CREATE DATABASE db_dotnet_product;
\c db_dotnet_product;

DROP TABLE IF EXISTS categories;
CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(50) UNIQUE,
    description VARCHAR(100),
    unique_id UUID DEFAULT gen_random_uuid(),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

DROP TABLE IF EXISTS products;
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    category_id INT NOT NULL,
    code VARCHAR(20) UNIQUE NOT NULL,
    product_name VARCHAR(50) NOT NULL,
    unit_price FLOAT NOT NULL DEFAULT 0,
    description VARCHAR(100),
    unique_id UUID DEFAULT gen_random_uuid(),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_categories FOREIGN KEY(category_id) REFERENCES categories(category_id)
);

DROP TABLE IF EXISTS images;
CREATE TABLE images (
    image_id SERIAL PRIMARY KEY,
    product_id INT NOT NULL,
    front_image VARCHAR(50),
    back_image VARCHAR(50),
    left_image VARCHAR(50),
    right_image VARCHAR(50),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
	updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_products FOREIGN KEY(product_id) REFERENCES products(product_id)
);

-- VIEWS 
-- view_product_data
DROP VIEW IF EXISTS view_product_data;
CREATE VIEW view_product_data AS 
SELECT pr.product_id, pr.unique_id,
    pr.product_name, pr.code,
    pr.unit_price, pr.description, 
    pr.created_at, pr.updated_at,
    ca.category_id, ca.category_name
FROM products pr
JOIN categories ca ON(ca.category_id = pr.category_id)
ORDER BY pr.created_at DESC;

