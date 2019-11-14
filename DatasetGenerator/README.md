This repository is used in order to split the Vygus 2018 Middle Egyptian dictionary into its constituent line entries to allow for visual analysis of the glyphs, and eventual Manuel de Codage reconstruction.
Secondly, this repository can be used to parse glyph locations from the PDFs to act as labels in case supervised learning is used for glyph detection. 

## Code
In DatasetGenerator, to produce the split PDF dataset, run the following code:

```c#
DatasetGenerator dg = new DatasetGenerator(LOCATION_OF_VYGUS_2018, DIRECTORY_OF_OUTPUT_FILES);
dg.ParsePdf();
```

In order to create the labels for ML, run:

```c#
var dlg = new DatasetLabelGenerator(DIRECTORY_OF_SPLIT_FILES);
dlg.ParseAllFiles();
```

In order to create the dataset of characters for training, run:

```c#
CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(LOCATION_OF_VYGUS_2018, DIRECTORY_FOR_OUTPUT_CHARS);
cdg.GetCharacterFilesFromPdf();
```