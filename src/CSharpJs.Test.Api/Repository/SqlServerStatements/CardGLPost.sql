select	T0."DocEntry"
    ,	T3."ItmsGrpNam" as "ItemGroupName"
    ,	T1."ItemCode"
    ,	T1."Dscription"
    ,	T1."Quantity"
    ,	T1."Price"
    ,	T1."LineTotal" as "Total"
    ,	T0."DocDate" as "TaxDate"
    ,	T0."CardCode"
    ,	T0."CardName"
    ,	T0."Serial"
    ,   T0."NumAtCard"
from OINV T0 
    inner join INV1 T1  on T0."DocEntry" = T1."DocEntry"
    inner join OITM T2  on T1."ItemCode" = T2."ItemCode"
    inner join OITB T3  on T2."ItmsGrpCod" = T3."ItmsGrpCod"
where T0."DocEntry" = @docEntry
    and T1."TreeType" in ('S', 'N')