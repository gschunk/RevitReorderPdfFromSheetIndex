# Revit Reorder PDF From Sheet Index
## An add-in for Autodesk Revit which re-orders an existing PDF from a sheet index.

### Dependencies:
 - Autodesk Revit .NET API
 - itext 5
 
### Usage

- Create a sheet index in Revit.
- Create a column in the sheet index for sorting. The data type must be Integer.
- Specify the relative sorting order. Actual sorting will be by relative sorting order and then by sheet number.
- Create a pdf with the sheets and order matching the sheet index.
- Start the add-in.
- Select the sheet index.
- Select the sort by column.
- Select the pdf file.
- Select "Reorder"
- The re-ordered pdf will open in the system default pdf viewer.
