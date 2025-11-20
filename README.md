I vårt projekt har jag ansvarat för att skapa och strukturera en modell som heter SongMetadata. 
Syftet med denna är att samla all viktig information om en låt på ett och samma ställe så att resten av programmet enkelt kan använda de.
SongMetadata innehåller olika fält som beskriver låten, till exempel
- Var filen ligger (filvägen)
- Låttitel
- Artist
- Album
- Vilket år låten släpptes
- Låtens läng

Jag valde att göra en egen klass för detta för att hålla projektet mer organiserat och tydligt.
Genom att samla metadata i en modell slipper vi repetera samma information på flera ställen i programmet, och andra delar av projektet kan hämta och visa låtdata utan problem.
Detta är en grundläggande del av hur musikfilerna hanteras i projektet och behövs för att allt annat ska fungera.

