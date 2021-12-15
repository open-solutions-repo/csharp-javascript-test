SELECT TOP 1 
CASE WHEN COALESCE(S1."PermId", SR."SUPERUSER") = 'Y' THEN 1 
WHEN S1."PermId" = ? AND S1."Permission" = 'F' Then 1
else 0 END as "Result"
from "OUSR" SR 
left join "USR3" S1  on SR."USERID" = S1."UserLink"
WHERE SR."USER_CODE" = ?
order by "Result" desc