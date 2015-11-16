CREATE TABLE dbo.Products
(
	ProductId UniqueIdentifier NOT NULL,
	[Asin] Char(10) NOT NULL,
	SearchIndex VarChar(25) NOT NULL,
	BrowseNode BigInt NOT NULL,
	ProductGroup VarChar(25) NOT NULL,
	Title VarChar(255) NOT NULL,
	Brand VarChar(50) NOT NULL,
	LargeImageUrl VarChar(255) NOT NULL,
	LowestNewPrice int NULL,
	DetailsPageUrl VarChar(255) NOT NULL,
	CustomerReviewsUrl VarChar(255) NOT NULL
)
