
merge Products as target
using UpdatedProducts as source
on (target.ProductID = source.ProductID)
when MATCHED and (target.ProductName <> source.ProductName or target.Rate <> source.Rate) then 
	Update SET target.ProductName = source.ProductName, target.Rate = source.Rate
	--Đã tồn tại trong source mà k có trong target
when not matched by target then
	insert (ProductID,ProductName,Rate)
	values (Source.ProductID, Source.ProductName,Source.Rate)

	--Đã tồn tại trong target mà k có trong source
when not matched by source then
delete

output $action,
DELETED.ProductID as TargetProductID,
DELETED.ProductName as TargetProductName,
DELETED.Rate as TargetRate,
INSERTED.ProductID as SourceProductID,
INSERTED.ProductName as SourceProductName,
INSERTED.Rate as SourceRate;

select @@ROWCOUNT;
