alter table dbo.Category
	add constraint DF_Category_CategoryId
	default NewSequentialId()
	for CategoryId
