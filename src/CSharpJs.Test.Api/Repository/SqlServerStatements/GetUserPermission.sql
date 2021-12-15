SELECT TOP 1 
CASE WHEN COALESCE(S1."PermId", SR."SUPERUSER") = 'Y' THEN 1 
WHEN S1."PermId" = @AuthorizationId AND S1."Permission" = 'F' Then 1
else 0 END as "Result"
from "OUSR" SR with(nolock)
left join "USR3" S1 with(nolock) on SR."USERID" = S1."UserLink"
WHERE SR."USER_CODE" = @UserName
order by "Result" desc