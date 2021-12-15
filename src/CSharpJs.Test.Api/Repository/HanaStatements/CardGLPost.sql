select	T0."DocEntry"
    ,	T0."DocDate" as "TaxDate"
    ,	T0."CardCode"
    ,	T0."CardName"
    ,	T0."Serial"
    ,   T0."NumAtCard"
    ,	T3."ItmsGrpNam" as "ItemGroupName"
    ,	T1."ItemCode"
    ,	T1."Dscription"
    ,	TO_DOUBLE(T1."Quantity")  as "Quantity"
    ,	TO_DOUBLE(T1."Price") as "Price"
    ,	TO_DOUBLE(T1."LineTotal") as "Total"
from OINV T0 
    inner join INV1 T1  on T0."DocEntry" = T1."DocEntry"
    inner join OITM T2  on T1."ItemCode" = T2."ItemCode"
    inner join OITB T3  on T2."ItmsGrpCod" = T3."ItmsGrpCod"
where T0."DocEntry" = ?
    and T1."TreeType" in ('S', 'N')