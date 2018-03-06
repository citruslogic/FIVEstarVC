select

year(StartDate),
COUNT(ProgramEventID)

from ProgramEvent
where ProgramTypeID = 4 
GROUP BY year(StartDate)
