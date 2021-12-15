SELECT
	V0."U_OPEN_CashCurrent" as "CashCurrent",
	COUNT(V1."DocEntry") as "SalesQuantity",
	COALESCE(SUM(V1."CashTotal"), 0) as "CashTotal",
	COALESCE(SUM(V1."DebitCardTotal"), 0) as "DebitCardTotal",
	COALESCE(SUM(V1."CreditCardTotal"), 0) as "CreditCardTotal",
	COALESCE(SUM(V1."AccountCreditTotal"), 0) as "AccountCreditTotal",
	COALESCE(SUM(V1."DocTotal"), 0) as "SaleTotal"
FROM "@OPEN_TD_CSHR" V0
LEFT JOIN (
	SELECT DISTINCT
		T0."POSCashN",
		T0."DocEntry",
		T0."DocNum", 
		COALESCE(T2."CashSum", 0) as "CashTotal",
		COALESCE(T3."CreditSum", 0) as "DebitCardTotal",
		COALESCE(T4."CreditSum", 0) as "CreditCardTotal",
		COALESCE((T0."DocTotal" - T0."PaidToDate"), 0) as "AccountCreditTotal",
		T0."DocTotal"
	FROM "OINV" T0
	INNER JOIN "INV1" T5 ON T0."DocEntry" = T5."DocEntry"
	LEFT JOIN "RCT2" T1 ON T0."DocEntry" = T1."DocEntry"
	LEFT JOIN "ORCT" T2 ON T1."DocNum" = T2."DocEntry"
	LEFT JOIN "RCT3" T3 ON T3."DocNum" = T2."DocEntry" AND T3."NumOfPmnts" = 1 -- Débito
	LEFT JOIN "RCT3" T4 ON T4."DocNum" = T2."DocEntry" AND T4."NumOfPmnts" > 1 -- Crédito
	INNER JOIN "@OPEN_TD_CSHR" T6 ON T0."POSCashN" = T6."DocEntry"
	WHERE T0."CANCELED" = 'N'
) V1 ON V0."DocEntry" = V1."POSCashN"
WHERE V0."DocEntry" = @DocEntry
GROUP BY V0."U_OPEN_CashCurrent"