#!/bin/bash

ROOT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

EXE="$ROOT_DIR/grammar/bin/Debug/net7.0/grammar"

GRAMMAR_DEF="$ROOT_DIR/kyloe/src/SyntaxAnalysis/grammar.def"
GENERATOR_JSON="$ROOT_DIR/kyloe/src/SyntaxAnalysis/generator.json"
OUT_DIR="$ROOT_DIR/kyloe/src/SyntaxAnalysis/Generated/"


"$EXE" "$GRAMMAR_DEF" generate "$GENERATOR_JSON" "$OUT_DIR"