Select 
	  "DocCode" as "Code"
	, "DocName" as "Name"
	, "U_OPEN_PrintFormat" as "PrintFormat"
From "RDOC"
Where "U_OPEN_Visible" = 'Y' and "U_OPEN_Type" = 2  and "DocName" = ?