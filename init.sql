DROP DATABASE IF EXISTS ecommerce;
CREATE DATABASE ecommerce;
USE ecommerce;

DROP TABLE IF EXISTS ITEM_CART;
DROP TABLE IF EXISTS CART;
DROP TABLE IF EXISTS ITEM;

CREATE TABLE ITEM (
    item_id INT PRIMARY KEY AUTO_INCREMENT,
    item_name VARCHAR(255) NOT NULL,
    item_category VARCHAR(255),
    item_description TEXT,
    item_price FLOAT NOT NULL,
    item_quantity INT NOT NULL,
    item_image_path VARCHAR(255) DEFAULT NULL
);

CREATE TABLE CART (
    cart_id VARCHAR(50) PRIMARY KEY,
    user_id INT NOT NULL
);

CREATE TABLE ITEM_CART (
    cart_id VARCHAR(50),
    item_id INT,
    quantity INT NOT NULL,
    PRIMARY KEY (cart_id, item_id),
    FOREIGN KEY (cart_id) REFERENCES CART(cart_id) ON DELETE CASCADE,
    FOREIGN KEY (item_id) REFERENCES ITEM(item_id) ON DELETE CASCADE
);

INSERT INTO `ITEM` VALUES (1,'Coca-Cola 12 Pack','Beverages','Refreshing carbonated soft drink.',6.99,1,'images/coca_cola.jpg'),(2,'Pepsi 2L Bottle','Beverages','Classic cola taste with a refreshing fizz.',2.99,1,'images/pepsi.jpg'),(3,'Orange Juice 1L','Beverages','100% pure orange juice with no added sugar.',3.49,1,'images/orange_juice.jpg'),(4,'Cold Brew Coffee','Beverages','Smooth and bold iced coffee.',4.99,1,'images/cold_brew.jpg'),(5,'Green Tea 500ml','Beverages','Antioxidant-rich green tea with natural flavors.',2.49,1,'images/green_tea.png'),(6,'Lemonade 1.5L','Beverages','Sweet and tangy fresh lemonade.',3.99,1,'images/lemonade.png'),(7,'Sparkling Water 1L','Beverages','Carbonated natural spring water.',1.99,1,'images/sparkling_water.png'),(8,'Chocolate Milk 500ml','Beverages','Creamy and rich chocolate-flavored milk.',2.79,1,'images/chocolate_milk.jpg'),(9,'Smoked Turkey Breast','Deli','Premium smoked turkey slices.',7.99,1,'images/smoked_turkey.png'),(10,'Cheddar Cheese Block','Deli','Aged cheddar cheese block for slicing.',5.49,1,'images/cheddar_cheese.jpg'),(11,'Salami Slices','Deli','Savory and spicy deli meat.',6.29,1,'images/salami.png'),(12,'Ham Slices','Deli','Classic ham slices for sandwiches.',5.99,1,'images/ham_slices.png'),(13,'Swiss Cheese Slices','Deli','Mild and nutty Swiss cheese.',4.99,1,'images/swiss_cheese.png'),(14,'Pastrami 200g','Deli','Spicy and smoked pastrami.',7.49,1,'images/pastrami.png'),(15,'Pepperoni Sticks','Deli','Spicy and cured pepperoni sticks.',6.99,1,'images/pepperoni_sticks.png'),(16,'Ribeye Steak','Meat','Juicy and tender ribeye steak.',14.99,1,'images/ribeye_steak.jpg'),(17,'Chicken Breast Fillets','Meat','Skinless, boneless chicken breast fillets.',9.99,1,'images/chicken_breast.png'),(18,'Ground Beef 1lb','Meat','Lean ground beef for cooking.',5.99,1,'images/ground_beef.jpg'),(19,'Pork Chops 1lb','Meat','Bone-in pork chops, tender and flavorful.',7.99,1,'images/pork_chops.jpg'),(20,'Salmon Fillet','Meat','Fresh Atlantic salmon fillet.',12.99,1,'images/salmon_fillet.png'),(21,'Bacon Strips 500g','Meat','Crispy and smoky bacon.',6.99,1,'images/bacon_strips.png'),(22,'Sausage Links 500g','Meat','Spiced pork sausages.',5.49,1,'images/sausage_links.png'),(23,'Whole Wheat Bread','Bread','Healthy whole wheat sandwich bread.',2.99,1,'images/whole_wheat_bread.png'),(24,'French Baguette','Bread','Crispy and fresh French-style baguette.',3.49,1,'images/french_baguette.jpg'),(25,'Bagels 6-Pack','Bread','Classic bagels perfect for breakfast.',4.29,1,'images/bagels.png'),(26,'Croissants 4-Pack','Bread','Flaky and buttery croissants.',5.99,1,'images/croissants.png'),(27,'Sourdough Loaf','Bread','Artisan sourdough bread.',4.99,1,'images/sourdough.png'),(28,'Garlic Bread','Bread','Oven-baked garlic butter bread.',3.99,1,'images/garlic_bread.jpg'),(29,'Gala Apples 3lb','Fruits','Fresh and crisp Gala apples.',4.99,1,'images/gala_apples.jpg'),(30,'Bananas 5-Pack','Fruits','Sweet and ripe bananas.',2.49,1,'images/bananas.jpg'),(31,'Strawberries 1lb','Fruits','Juicy and sweet strawberries.',3.99,1,'images/strawberries.png'),(32,'Blueberries 1lb','Fruits','Antioxidant-rich fresh blueberries.',5.99,1,'images/blueberries.png'),(33,'Pineapple Whole','Fruits','Sweet and tropical whole pineapple.',3.99,1,'images/pineapple.jpg'),(34,'Mango 2-Pack','Fruits','Ripe and juicy mangoes.',4.49,1,'images/mango.jpg'),(35,'Watermelon Whole','Fruits','Large and refreshing watermelon.',7.99,1,'images/watermelon.jpg'),(36,'Pain Reliever 24ct','Pharmacy','Acetaminophen tablets for pain relief.',6.99,1,'images/pain_reliever.jpg'),(37,'Cough Syrup 250ml','Pharmacy','Effective cough relief formula.',8.49,1,'images/cough_syrup.jpg'),(38,'Vitamin C Gummies','Pharmacy','Immunity-boosting vitamin C gummies.',10.99,1,'images/vitamin_c.jpg'),(39,'Allergy Relief 30ct','Pharmacy','Antihistamine tablets for allergy relief.',9.99,1,'images/allergy_relief.jpg'),(40,'Antacid Tablets 50ct','Pharmacy','Fast relief from acid reflux and heartburn.',7.49,1,'images/antacid.jpg'),(41,'Eye Drops 15ml','Pharmacy','Soothing eye drops for dry eyes.',5.99,1,'images/eye_drops.png'),(42,'Whole Milk 1L','Dairy','Rich and creamy whole milk.',2.99,1,'images/whole_milk.png'),(43,'Greek Yogurt 500g','Dairy','Thick and protein-rich Greek yogurt.',4.99,1,'images/greek_yogurt.jpg'),(44,'Cheddar Cheese Slices','Dairy','Sliced cheddar cheese for sandwiches.',3.99,1,'images/cheddar_slices.jpg'),(45,'Butter 500g','Dairy','Creamy and rich butter.',4.99,1,'images/butter.jpg'),(46,'Mozzarella Cheese Block','Dairy','Perfect for homemade pizzas.',5.99,1,'images/mozzarella.jpg'),(47,'Cottage Cheese 250g','Dairy','Low-fat cottage cheese.',3.49,1,'images/cottage_cheese.jpg'),(48,'Protein Powder 2lb','Health','High-quality whey protein for fitness.',29.99,1,'images/protein_powder.jpg'),(49,'Multivitamin 60ct','Health','Essential vitamins and minerals for daily health.',14.99,1,'images/multivitamin.jpg'),(50,'Herbal Tea Pack','Health','Assorted herbal teas for relaxation.',6.99,1,'images/herbal_tea.jpg'),(51,'Fish Oil 120ct','Health','Omega-3 rich fish oil soft gels.',15.99,1,'images/fish_oil.jpg'),(52,'Potato Chips Large','Snack','Crispy and salty potato chips.',3.49,1,'images/potato_chips.png'),(53,'Chocolate Bar','Snack','Rich and creamy milk chocolate bar.',2.99,1,'images/chocolate_bar.jpg'),(54,'Trail Mix 500g','Snack','Healthy mix of nuts, fruits, and chocolate.',7.49,1,'images/trail_mix.png');