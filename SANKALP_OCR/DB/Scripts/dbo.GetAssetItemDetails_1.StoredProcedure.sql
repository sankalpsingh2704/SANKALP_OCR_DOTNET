USE [IQInvoiceItem]
GO
/****** Object:  StoredProcedure [dbo].[GetAssetItemDetails_1]    Script Date: 17-05-2017 19:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetAssetItemDetails_1]
as

create table #IDs(
pid int IDENTITY (1,1),
id int
)
declare @indexTbl TABLE (pid int,id int,i_index int)
declare @tempCodes TABLE (Id int ,code  varchar(10), indexNo int)
declare @results varchar(max)
select @results = coalesce(@results + '', '') +  replicate(cast(ID as varchar(10)) + ',',Round(convert(float , ISNULL(Qty,0)),0))
from orderitemdetails where asset=1
--order by col
PRINT @results

insert into #IDs(id) select Name from dbo.splitstring(@results)
insert into @indexTbl(pid,id,i_index) Select pid,id,ROW_NUMBER() OVER(PARTITION BY [id] ORDER BY [id]) from #IDs
--insert into @tempCodes(Id,code,indexNo) select i.id,oid.Code,ROW_NUMBER() OVER(PARTITION BY oid.LastCodeIndex ORDER BY oid.LastCodeIndex) from orderitemdetails oid inner join @indexTbl i on oid.ID = i.id
--select * from @tempCodes
insert into @tempCodes(Id,code,indexNo)
SELECT i.id, Code,
    s_index = [dbo].[fn_GetCodeIndex] (code) +  ROW_NUMBER() OVER(PARTITION BY [Code] ORDER BY [Code])
	--,    t_index = DENSE_RANK() OVER (ORDER BY [Code])
	from orderitemdetails oid inner join #IDs i on oid.ID = i.id --and Code is not null
select 
oid.ID as [Maintenance Object ID]
,oid.ItemName as [Category (Reference)]
--,LEFT(oid.ItemName, 2) + CAST(i.i_index as varchar(20)) as [Code]
,ISNULL(oid.Code, 'DEFAULT') + CAST(i.indexNo as varchar(20)) as [Code]
,oid.ItemName as [Reference]
,oid.ItemName as [Description]
,'' as [Remarks]	
,'' as [Prevention Notes]
,'Technical Object' as [Class]
,'EXISTING' as [Status (Code)]
,'NA' as [Default Nature (Code)]
,'' as [Maintenance Object Group (Code)]
,'' as [Serial Number]
,'' as [Brand]
,'' as [Power]	
,'' as [Bluetooth Tag]
,'' as [Warranty]
,'' as [Warranty Date]
---,'SUP_SSMSPL' as [Supplier (Code)]
,CASE WHEN S.SupplierName IS NULL THEN 'n/a' ELSE S.Code END as [Supplier (Code)]
,'Jigni' as [Site (Reference)]	
,'Ground Floor' as [Building (Reference)]
,'' as [Floor (Reference)]
,'' as [Room (Reference)]	
,'' as [Land (Reference)]	
,'' as [Outside Location (Reference)]
,'' as [Energy Object (Reference)]
,'' as [Stock (Description)]
,'' as [Symbol (Reference)]
,'' as [Classification (Code)]
,'' as [Planner Type]
,'' as [Default Planner (Corporate Id)]
,'' as [Operator Type]
,'' as [Default Operator (Key Field Value)]
,'' as [Default Task Force (Code)]
,'' as [Default Project (Code)]
,'' as [Default Project Part (Code)]
,'' as [Cost Center (Code)]
,'' as [Cost Center Fiscal Entity (Code)]
,'' as [Default Cost Category (Code)]
,'' as [Investment Date]
,'' as [Construction Date]
,'' as [Expected Lifetime (Years)]
,'' as [Overridden Replacement Year]
,'' as [Expected Replacement Amount]
,'' as [Expected Replacement Currency]
,'' as [Yearly Cost]
,'' as [Yearly Cost Currency]
,'' as [Contract Code]
,'' as [Contract Reference]
,'' as [Contract Status (Reference)]
,'' as [Contract Number]
,'' as [Contractor (Code)]
,'' as [Contact Person 1]	
,'' as [Contract Remark 1]	
,'' as [Contact Person 2]	
,'' as [Contract Remark 2]	
,'' as [Scan Code]
,'' as [Criticality (Code)]
,'' as [Access Instruction]	
,o.InvoiceNo as [udi.U_INVOICE_NO_]	
,o.PO as [udi.U_PO_NUMBER]
,o.InvoiceDate as [udi.U_PO_DATE]
,'' as [udi.U_INITIAL_BOOK_VALUE]
,'Purchased' as [udi.U_OBTAINED_BY]
,'INR' as [udi.U_CURRENCY]
,o.InvoiceDate as [udi.U_DATE_BOUGHT]
,oid.Price as [udi.U_LEASE_AMOUNT]	
,'' as [udi.U_END_LEASE_DATE]


--o.Vendor, o.InvoiceNo, o.InvoiceDate, o.PO, o.Amount, o.PAN, o.BUYERCSTNO,
--o.BUYERVATTIN, o.COMPANYCSTNO, o.COMPANYVATTIN,
--oid.ItemName, oid.Price, oid.Qty,oid.Tax, oid.Total, oid.asset
from orderitemdetails oid inner join @tempCodes i on oid.ID = i.id
inner join OrderItems o on o.ID = oid.orderitemid 
LEFT JOIN Suppliers S ON S.SupplierName= o.Vendor
where oid.asset=1
drop table #IDs

GO
