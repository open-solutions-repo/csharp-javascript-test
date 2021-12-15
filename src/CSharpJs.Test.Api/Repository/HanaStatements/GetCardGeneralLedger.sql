select	T0."DocEntry"
    ,	1 as [Status]
    ,	T3."ItmsGrpNam" as "ItemGroupName"
    ,	TO_DOUBLE(SUM(T1.LineTotal)) as "DocTotal"
    ,	T0."DocDate"
    ,	T0."DocDueDate" as "DueDate"
    ,	TO_DOUBLE(T6."InsTotal" - T6."PaidToDate") as "BalToDue"
    ,	T4."CardCode"
    ,	T0."TaxDate"
    ,	T0."Serial"
    ,	TO_DOUBLE(CASE 
                    when T6."InsTotal" - T6."PaidToDate" > 0 then NULL
			        when T6."InsTotal" - T6."PaidToDate" <= 0 then T7."TaxDate"
			            else NULL end) as "PaidDate"
from OINV T0
    inner join INV1 T1 on T0."DocEntry" = T1."DocEntry"
    inner join OITM T2 on T1."ItemCode" = T2."ItemCode"
    inner join OITB T3 on T2."ItmsGrpCod" = T3."ItmsGrpCod"
    inner join OCRD T4 on T0."CardCode" = T4."CardCode"
    inner join INV6 T6 on T0."DocEntry" = T6."DocEntry"
    left join RCT2 T5 on T6."DocEntry" = T5."DocEntry" and T6."InstlmntID" = T5."InstId"
    left join ORCT T7 on T5."DocNum" = T7."DocEntry"
where MONTH(T0."DocDate") = ?
	and YEAR(T0."DocDate") = ?
	and T4."CardCode" = ?
group by    T0."DocEntry"
        ,   T3."ItmsGrpNam"
        ,   T0."DocDate"
        ,   T0."DocDueDate"
        ,   T6."InsTotal" - T6."PaidToDate"
        ,   T4."CardCode"
        ,   T0."TaxDate"
        ,   T0."Serial"
        ,   CASE 
                when T6."InsTotal" - T6."PaidToDate" > 0 then NULL 
                when T6."InsTotal" - T6."PaidToDate" <= 0 then T7."TaxDate" 
                    else NULL end