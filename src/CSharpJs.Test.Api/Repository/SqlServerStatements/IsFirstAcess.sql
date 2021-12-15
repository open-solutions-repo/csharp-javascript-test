select
    (select COUNT(*) from[@XNET_OM_OSOR] with(nolock)) as OsOpen
, (select COUNT(*) from[OWOR] with(nolock)) as OpOpen
, (select COUNT(*) from "@XNET_OM_OPRJ" with(nolock)) as "Projects"
, (select COUNT(*) from[@XNET_OM_OSOR] with(nolock) where U_OM_Status != 4 and U_OM_Status != 5 and U_OM_Status != 6) as OsReleased
, (select COUNT(*) from[@XNET_OM_OGGF] with(nolock)) as Ggf
, (select COUNT(*) from[@XNET_OM_OMSO] with(nolock)) as MaskServiceOrder
, (select COUNT(*) from[@XNET_OM_OMOP] with(nolock)) as MaskRouting
, (select COUNT(*) from[@XNET_OM_OSIP] with(nolock)) as MaskInspection
, (select COUNT(*) from[@XNET_OM_OQTT] with(nolock)) as QuotaType
, (select COUNT(*) from[@XNET_OM_OTGP] with(nolock)) as InstrumentGroup
, (select COUNT(*) from[@XNET_OM_OTOL] with(nolock) where U_OM_Active = 'Y') as Instrument
, (select COUNT(*) from[@XNET_OM_OTLC] with(nolock)) as InstrumentLocal
, (select COUNT(*) from[@XNET_OM_ORFS] with(nolock)) as ReasonForStopping
, (select COUNT(*) from[@XNET_OM_TMEP] with(nolock)) as TimeEntryParameter
, (select COUNT(*) from[@XNET_OM_OWST] with(nolock)) as OrderType
, (select COUNT(*) from[@XNET_OM_FRNC] with(nolock)) as Familyrnc
, (select COUNT(*) from[@XNET_OM_PRNC] with(nolock)) as Phasernc
, (select COUNT(*) from[@XNET_OM_ARNC] with(nolock)) as accountrnc
, (select COUNT(*) from[@XNET_OM_DRNC] with(nolock)) as destinyrnc
, (select COUNT(*) from[@XNET_OM_LRNC] with(nolock)) as dispositionrnc
, (select COUNT(*) from[@XNET_OM_TRNC] with(nolock)) as typernc
, (select COUNT(*) from[@XNET_OM_SRNC] with(nolock)) as statusrnc
, (select COUNT(*) from[@XNET_OM_ORNC] with(nolock)) as Nonconformity
, (select COUNT(*) from[@XNET_OM_OPRC] with(nolock)) as pricing
, (select COUNT(*) from "@XNET_OM_OSHP" with(nolock)) as "Format"
, (select COUNT(*) from "OHEM" with(nolock)) as "Contributor"
, (select COUNT(*) from "@XNET_OM_SHFT" with(nolock)) as "Shift"
, (select COUNT(*) from "@XNET_OM_NOTE" with(nolock)) as "Note"
, (select COUNT(*) from "@XNET_OM_QLRT" with(nolock)) as "QualityRequirements"
, (select COUNT(*) from [@XNET_OM_OSID] with(nolock)) as Inspectionentry
, (select COUNT(*) from "@XNET_OM_OCLT" with(nolock)) as "Collector"
, (select COUNT(*) from OIGE OG with(nolock)
	inner join "IGE1" G1 with(nolock) on OG."DocEntry" = G1."DocEntry"
	where not G1."U_OM_OrderEntry" is null) as "IssueMaterial"
, (select COUNT(*) from [OIGN] T0 with(nolock)
	inner join "IGN1" T1 with(nolock) on T0."DocEntry" = T1."DocEntry"
	where T1."BaseType" = 202 
	and T1."BaseRef" <> '')
	as "ProductEntry"
, (select COUNT(*) from [@XNET_OM_OGFC] with(nolock)) as "GGFClassif"
, (select COUNT(*) from "@XNET_OM_OFIC" with(nolock)) as "Fic"
, (select COUNT(*) from "@XNET_OM_OING" with(nolock)) as "instructionGroup"
, (select COUNT(*) from "@XNET_OM_OSUB" with(nolock)) as "Subset"
, (select COUNT(*) from "@XNET_OM_OQUT" with(nolock)) as "SalesQuotation"
, (SELECT CASE "U_OM_UseFic"
	        WHEN 'Y' THEN 1
	        ELSE 0 END AS "U_OM_UseFic"
        FROM "@XNET_OM_CONF" with(nolock)) AS "VisibleFic"
, (SELECT COUNT(*)
    FROM "@XNET_OM_OSOR" T0 with(nolock) 
    LEFT JOIN "RDR1"  T1 with(nolock) ON T0."U_OM_OriginAbs" = T1."DocEntry" 
    AND T0."U_OM_NumAtCard" = T1."U_OM_NumberOC" AND T0."U_OM_NumAtCard2" = T1."U_OM_NumberOCLine"
    WHERE T0."U_OM_Status" = 3  AND  coalesce(T1."LineStatus", 'O') = 'O') as "OsNotBilled"
,(select count(*)
	from "OPOR" T0 with(nolock)
	inner join "POR1" T1 with(nolock) on T0."DocEntry" = T1."DocEntry"
	where T1."U_OM_OrderType" is not null
	and T0."CANCELED" = 'N') as SendTo3
, (select COUNT(*) from "@XNET_OM_STGP" with(nolock)) as "SetGroup"
, (select COUNT(*) from "@XNET_OM_OCMP" with(nolock)) as "Components"
, (select U_OM_Level from OUSR with(nolock) WHERE USER_CODE = @UserName) as UserLevel
, (SELECT COUNT(*) FROM "@XNET_OM_OEXP" WITH(NOLOCK) WHERE "U_OM_Status" = '0') AS "ExpeditionPacking"
, (SELECT COUNT(*) FROM "@XNET_OM_OBOX" WITH(NOLOCK)) AS "ExpeditionBox"
, (select COUNT(*) from RDOC with(nolock) where "U_OM_VisibleOPEN" = 'Y' and "U_OM_TypeOPEN" = 2) As "Reports"
, (select "QtyDec" from OADM with(nolock)) as "QtyDec"
, (select COUNT(*) from "@XNET_OM_CONF" with(nolock) WHERE "U_OM_UseCatalogNumber" = 'Y') AS "UseCatalogNumber"
, (Select Count(*) From "@XNET_OM_OMOP" with(nolock) Where "U_OM_Status" = 1) As "MasksRevision"