## MetadataReader - min del av projektet (Del 2)
I den här delen av projektet har jag jobbat med klassen MetadataReader.
Det är en klass som läser in metadata från musikfiler, till exempel MP3-filer.
MetadataReader hämtar information som:
- Titel
- Artist
- Album
- Årtal
- Längd på låten
Jag använder bibliotek **TagLib** för att kunna läsa metadata filerna.
Om en fil saknar metadata eller om något går fel, så används standardvärden så att programmet inte kraschar .

Detta gör det enkelt för resten av projektet att få ut ett färdigt `SongMetadata`-objekt för varje låt.
