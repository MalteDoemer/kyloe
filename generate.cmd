@echo off

SET "ROOT_DIR=%~dp0"


SET "EXE=%ROOT_DIR%grammar\bin\Debug\net7.0\grammar.exe"

SET "GRAMMAR_DEF=%ROOT_DIR%kyloe\src\SyntaxAnalysis\grammar.def"
SET "GENERATOR_JSON=%ROOT_DIR%kyloe\src\SyntaxAnalysis\generator.json"
SET "OUT_DIR=%ROOT_DIR%kyloe\src\SyntaxAnalysis\Generated"

"%EXE%" "%GRAMMAR_DEF%" generate "%GENERATOR_JSON%" "%OUT_DIR%"