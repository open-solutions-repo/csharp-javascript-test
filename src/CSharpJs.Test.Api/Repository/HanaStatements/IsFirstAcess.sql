select
(select COUNT(*) from "@XNET_OM_OSOR" ) as "OsOpen"
, (select COUNT(*) from "OWOR") as "OpOpen"
, (select COUNT(*) from "@XNET_OM_OPRJ") as "Projects"
, (select COUNT(*) from "@XNET_OM_OSOR" where "U_OM_Status" != 4 and "U_OM_Status" != 5 and "U_OM_Status" != 6) as "OsReleased"
, (select COUNT(*) from "@XNET_OM_OGGF" ) as "Ggf"
, (select COUNT(*) from "@XNET_OM_OMSO" ) as "MaskServiceOrder"
, (select COUNT(*) from "@XNET_OM_OMOP" ) as "MaskRouting"
, (select COUNT(*) from "@XNET_OM_OSIP" ) as "MaskInspection"
, (select COUNT(*) from "@XNET_OM_OQTT" ) as "QuotaType"
, (select COUNT(*) from "@XNET_OM_OTGP" ) as "InstrumentGroup"
, (select COUNT(*) from "@XNET_OM_OTOL" where "U_OM_Active" = 'Y') as "Instrument"
, (select COUNT(*) from "@XNET_OM_OTLC" ) as "InstrumentLocal"
, (select COUNT(*) from "@XNET_OM_ORFS" ) as "ReasonForStopping"
, (select COUNT(*) from "@XNET_OM_TMEP" ) as "TimeEntryParameter"
, (select COUNT(*) from "@XNET_OM_OWST" ) as "OrderType"
, (select COUNT(*) from "@XNET_OM_FRNC" ) as "Familyrnc"
, (select COUNT(*) from "@XNET_OM_PRNC" ) as "Phasernc"
, (select COUNT(*) from "@XNET_OM_ARNC" ) as "Accountrnc"
, (select COUNT(*) from "@XNET_OM_DRNC") as "Destinyrnc"
, (select COUNT(*) from "@XNET_OM_LRNC" ) as "Dispositionrnc"
, (select COUNT(*) from "@XNET_OM_TRNC" ) as "Typernc"
, (select COUNT(*) from "@XNET_OM_SRNC" ) as "Statusrnc"
, (select COUNT(*) from "@XNET_OM_ORNC" ) as "Nonconformity"
, (select COUNT(*) from "@XNET_OM_OPRC" ) as "Pricing"
, (select COUNT(*) from "@XNET_OM_OSHP" ) as "Format"
, (select COUNT(*) from "OHEM" ) as "Contributor"
, (select COUNT(*) from "@XNET_OM_SHFT") as "Shift"
, (select COUNT(*) from "@XNET_OM_NOTE") as "Note"
, (select COUNT(*) from "@XNET_OM_QLRT") as "QualityRequirements"
, (select COUNT(*) from "@XNET_OM_OSID") as "Inspectionentry"
, (select COUNT(*) from "@XNET_OM_OSID") as Inspectionentry
, (select COUNT(*) from "@XNET_OM_OCLT") as "Collector"
, (select COUNT(*) from OIGE OG 
	inner join "IGE1" G1 on OG."DocEntry" = G1."DocEntry"
	where not G1."U_OM_OrderEntry" is null) as "IssueMaterial"
, (select COUNT(*) from "OIGN" T0
	inner join "IGN1" T1 on T0."DocEntry" = T1."DocEntry"
	where T1."BaseType" = 202 
	and T1."BaseRef" <> '')
	as "ProductEntry"
, (select COUNT(*) from "@XNET_OM_OGFC") as "GGFClassif"
, (select COUNT(*) from "@XNET_OM_OFIC") as "Fic"
, (select COUNT(*) from "@XNET_OM_OING") as "instructionGroup"
, (select COUNT(*) from "@XNET_OM_OSUB") as "Subset"
, (select COUNT(*) from "@XNET_OM_OQUT") as "SalesQuotation"
, (SELECT CASE "U_OM_UseFic"
	        WHEN 'Y' THEN 1
	        ELSE 0 END AS "U_OM_UseFic"
        FROM "@XNET_OM_CONF") AS "VisibleFic"
, (SELECT COUNT(*)
    FROM "@XNET_OM_OSOR" T0  
    LEFT JOIN "RDR1" T1 ON T0."U_OM_OriginAbs" = T1."DocEntry" 
    AND T0."U_OM_NumAtCard" = T1."U_OM_NumberOC" AND T0."U_OM_NumAtCard2" = T1."U_OM_NumberOCLine"
    WHERE T0."U_OM_Status" = 3  AND  COALESCE(T1."LineStatus", 'O') = 'O') as "OsNotBilled"
,(select count(*)
	from "OPOR" T0 
	inner join "POR1" T1 on T0."DocEntry" = T1."DocEntry"
	where T1."U_OM_OrderType" is not null
	and T0."CANCELED" = 'N') as "SendTo3"
--, (select COUNT(*)
--    from  "@XNET_OM_OSID" T0 
--    inner join "@XNET_OM_OPR2" T1  on T0."DocEntry" = T1."U_OM_OisdEntry"
--    inner join "@XNET_OM_OOPR" T2  on T1."DocEntry" = T2."DocEntry"
--    inner join "@XNET_OM_SOR4" T3  on T1."DocEntry" = T3."U_OM_OoprEntry"
--    inner join "@XNET_OM_OSOR" T4  on T3."DocEntry" = T4."DocEntry"
--	    and (T4."U_OM_Status" = 1 OR T4."U_OM_Status" = 2) 
--	    and T0."U_OM_Quantity" <> 
--							    (case T2."U_OM_IsRework"
--							    when 'Y' then (select S1."U_OM_QttRejected" from "@XNET_OM_OPR2" S0
--											    inner join "@XNET_OM_OSID" S1 on S0."U_OM_OisdEntry" = S1."DocEntry" and S0."DocEntry" = T2."U_OM_FatherProcessRework")
--							    else T4."U_OM_PlannedQty" end)) as "InspectionPartialOpen"
--   , (select ((SELECT COUNT(*)
--            FROM "@XNET_OM_CONF" Z 
--            CROSS JOIN "OPDN" T0 
--            INNER JOIN "PDN1" T1  ON T0."DocEntry" = T1."DocEntry" and T0."CANCELED" = 'N' AND T0."CreateDate" >= COALESCE(Z."U_OM_CutDateInspectionEntry", '1900-01-01')
--            INNER JOIN "OITM" T2  ON T1."ItemCode" = T2."ItemCode" and T2."U_OM_Inspect" = 'Y'
--           AND NOT EXISTS( select 1 from "@XNET_OM_OSID" 
--            where "U_OM_ItemCode" = T2."ItemCode" and "U_OM_OriginEntry" = T0."DocEntry" and "U_OM_OriginType" = T0."ObjType"
--            and "U_OM_OriginLineNum" = T1."LineNum")))
--            +
--            ( SELECT COUNT(*)
--            FROM "@XNET_OM_CONF" Z 
--            CROSS JOIN "OPCH" T0 
--            INNER JOIN "PCH1" T1  ON T0."DocEntry" = T1."DocEntry" and T0."CANCELED" = 'N' AND T0."CreateDate" >= COALESCE(Z."U_OM_CutDateInspectionEntry", '1900-01-01')
--            INNER JOIN "OITM" T2  ON T1."ItemCode" = T2."ItemCode" and T2."U_OM_Inspect" = 'Y'
--            AND NOT EXISTS( select 1 from "@XNET_OM_OSID" 
--            where "U_OM_ItemCode" = T2."ItemCode" and "U_OM_OriginEntry" = T0."DocEntry" and "U_OM_OriginType" = T0."ObjType"
--            and "U_OM_OriginLineNum" = T1."LineNum")) FROM DUMMY) as "InspectionEntryOpen"
,(select COUNT(*) from "@XNET_OM_STGP") as "SetGroup"
,(select COUNT(*) from "@XNET_OM_OCMP") as "Components"
            --          + 

		--(	SELECT T0.*
		--	FROM "@XNET_OM_CONF" Z 
		--		CROSS JOIN OWOR T0 
		--		LEFT JOIN OCRD T1  ON T0."CardCode" = T1."CardCode" AND T0."CreateDate" >= COALESCE(Z."U_OM_CutDateFinalInspection", '1900-01-01')
		--		INNER JOIN IGN1 T2  ON T0."DocEntry" = T2."BaseEntry" and T0."ObjType" = T2."BaseType"
		--		INNER JOIN OIGN T3  ON T2."DocEntry" = T3."DocEntry"
		--		INNER JOIN OITM T4  ON T0."ItemCode" = T4."ItemCode" and T4."ManBtchNum" = 'Y' and T4."ManSerNum" = 'N' and T4."U_OM_Inspect" = 'Y'
		--								AND NOT EXISTS( select 1 from "@XNET_OM_OSID" 
  --          where "U_OM_ItemCode" = T2."ItemCode" and "U_OM_OriginEntry" = T0."DocEntry" and "U_OM_OriginType" = T0."ObjType"
             --   and "U_OM_OriginLineNum" = -1)
				--)
--, (select COUNT(*)
--    from "@XNET_OM_OSOR" T0 
--    INNER JOIN "@XNET_OM_SOR4" T1 ON T0."DocEntry" = T1."DocEntry"
--    INNER JOIN "@XNET_OM_OOPR" T2 ON T1."U_OM_OoprEntry" = T2."DocEntry"
--    WHERE (T0."U_OM_Status" = 1 OR T0."U_OM_Status" = 2) AND T2."U_OM_IsFinishi" = 'N'
--    and (SELECT COUNT(1) FROM "@XNET_OM_OTME" WHERE "U_OM_Wor4Entry" = T2."DocEntry" and "U_OM_Status" = 1) >
--    (SELECT COUNT(1) FROM "@XNET_OM_OTME" WHERE "U_OM_Wor4Entry" = T2."DocEntry" and "U_OM_Status" = 3)) as "ProcessOpen"
--, (select COUNT(*) from "OIGE" 
--    where not "U_OM_Employee" is null and not "U_OM_WorkOrder" is null) as "IssueMaterial"
, (select "U_OM_Level" from "OUSR" WHERE "USER_CODE" = ?) as "UserLevel" 
, (SELECT COUNT(*) FROM "@XNET_OM_OEXP" WHERE "U_OM_Status" = '0') AS "ExpeditionPacking"
, (SELECT COUNT(*) FROM "@XNET_OM_OBOX" ) AS "ExpeditionBox"
, (select COUNT(*) from "RDOC" where "U_OM_VisibleOPEN" = 'Y' and "U_OM_TypeOPEN" = 2) As "Reports"
, (select "QtyDec" from OADM) as "QtyDec"
, (select COUNT(*) from "@XNET_OM_CONF" WHERE "U_OM_UseCatalogNumber" = 'Y') AS "UseCatalogNumber"
, (Select Count(*) From "@XNET_OM_OMOP" Where "U_OM_Status" = 1) As "MasksRevision"
FROM dummy