/*
Scorched earth:
delete from BoxServer.Boxs

select * from BoxServer.Boxs where Description = '~~bootstrap~~'
select count(*) Boxs from BoxServer.Boxs
*/
delete from BoxServer.Boxs where Description = '~~bootstrap~~'
PRINT 'Deletes complete'

INSERT INTO BoxServer.Boxs
(BoxId, Name, Description, Active)
VALUES
 ('7D259C54-3272-4147-B292-F77046211AED','Box A','~~bootstrap~~',1)
,('2194D6D4-0398-41D9-946E-038B88D30152','Box B','~~bootstrap~~',1)
,('622F4226-F2B0-4843-83B4-A536D1141597','Box C','~~bootstrap~~',1)
,('6100307D-3680-4369-BA56-320E3B50D0DA','Box D','~~bootstrap~~',1)
,('4B65BCBC-5923-4233-858A-295A2248C34D','Box E','~~bootstrap~~',1)
,('ECDA37A0-94E3-4E21-802F-8E2045C4CD32','Box F','~~bootstrap~~',1)
