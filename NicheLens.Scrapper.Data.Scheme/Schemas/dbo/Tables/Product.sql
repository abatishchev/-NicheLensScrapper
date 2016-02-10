create table dbo.Product
(
	ProductId UniqueIdentifier not null,
	[Asin] Char(10) not null,
	SearchIndex VarChar(25) not null,
	BrowseNode BigInt not null,
	ProductGroup VarChar(25) not null,
	Title VarChar(255) not null,
	Brand VarChar(50) null,
	LargeImageUrl VarChar(255) null,
	LowestNewPrice int null,
	DetailsPageUrl VarChar(255) not null,
	CustomerReviewsUrl VarChar(255) not null,

	CategoryId  UniqueIdentifier not null
)
