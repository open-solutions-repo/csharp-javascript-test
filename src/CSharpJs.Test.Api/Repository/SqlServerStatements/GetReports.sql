Select 
	  "DocCode" as "Code"
	, "DocName" as "Name"
	, COALESCE("Notes", "DocName") as "AliasName"
	, "U_OPEN_PrintFormat" as "PrintFormat"
From RDOC WITH(NOLOCK)
Where "U_OPEN_Visible" = 'Y' and "U_OPEN_Type" = 2
