create table dbo.Category
(
	CategoryId UniqueIdentifier not null,
	Name VarChar(255) not null,
	NodeId BigInt not null,
	ParentNodeId BigInt not null,
	SearchIndex VarChar(50) not null
)