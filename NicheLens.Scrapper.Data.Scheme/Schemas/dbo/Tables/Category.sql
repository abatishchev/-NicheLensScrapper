create table dbo.Category
(
	CategoryId UniqueIdentifier not null,
	NodeId BigInt not null,
	ParentNodeId BigInt not null,
	SearchIndex VarChar(24) not null,
	Name VarChar(max) not null,
)