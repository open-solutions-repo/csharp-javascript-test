UPDATE "@XNET_OM_SOR9" SET "U_OM_HasMngInspection" = @Value
where "DocEntry" = @DocEntry 
and "LineId" = @LineId