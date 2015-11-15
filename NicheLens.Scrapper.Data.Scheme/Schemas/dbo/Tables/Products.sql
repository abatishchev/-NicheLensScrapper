CREATE TABLE dbo.Products
(
	ProductId UniqueIdentifier NOT NULL,
	[Asin] varchar(10) NOT NULL,
	SearchIndex varchar(25) NOT NULL,
	BrowseNode int NOT NULL,
	ProductGroup varchar(25) NOT NULL,
	Title varchar(255) NOT NULL,
	Brand varchar(50) NOT NULL,
	LargeImageUrl varchar(255) NOT NULL,
	LowestNewPrice int NOT NULL,
	DetailsPageUrl varchar(255) NOT NULL,
	CustomerReviewsUrl varchar(255) NOT NULL
)
