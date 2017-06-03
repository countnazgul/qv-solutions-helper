Set QV  = CreateObject("QlikTech.QlikView")
Set fso  = CreateObject("Scripting.FileSystemObject")
Set colArgs = WScript.Arguments.Named

set file = fso.OpenTextFile(colArgs.Item("tempfile"), 1)
qvscript = file.ReadAll

filePath = colArgs.Item("qvw")

Set f = QV.OpenDocEx(filePath, 1, false, "", "", "", True)
set docprop = f.GetProperties
docprop.Script = qvscript
f.SetProperties docprop

f.Save

saveEmpty = colArgs.Item("saveempty")

if saveEmpty = "true" then 
  f.RemoveAllData
  f.SaveAs Replace(filePath, ".qvw", "_NoData.qvw")
end if

f.CloseDoc

'QV.Quit
WScript.Quit