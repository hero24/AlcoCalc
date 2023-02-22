# AlcoCalc
Simple brewers calculator with GUI written in C#.

AlcoCalc is a brewers calculator that allows its users to:
- Calculate ABV of diluted drink
- Calculate volumes required to achieve drink of given strength
- Calculate how much water to dilute drink with to achieve given strength
- Get ABV of brew from given starting and finishing sugar gravity
- Correct gravity reading from given temperature to 20*C
- Convert sugar gravity to Blg
- Build a 'starting gravity' of must taking into account changes between sugar additions
- Calculate ABV of a cocktail or mixed drink
- Store notes
- Store projects - with details and calculate data as brew progresses
- Calculate Irish and Uk units of a drink
- lbx version can generate label files for Brother VC-500W printer:
* a small 25x25 name + abv beerstamp
* ingredients label for wine with abv and date


Project has 3 branches:
- `main` - a stable version
- `next-experimental` - experimental next version were dev work is made - things might change here without notices
- `nxt-exp-lbx` - experimental branch for lbx version
- `exp-lbx` - this is where lbx version of program resides, it allows for generating brother vc-500w lbx label files. This is DIY build.
