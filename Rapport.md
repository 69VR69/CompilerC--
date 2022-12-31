# Liste des fonctionnalit�s du compilateur
## Fonctionnalit�s essentielles
* Compile les expressions
* Compile les conditionelles
* Compile les variables
* Compile les boucles
* Compile les fonctions
* Compile les pointeurs et tableaux
* Poss�de un runtime pour les fonction `print()`, `malloc()` et `free()`

## Fonctionnalit�s suppl�mentaires
* Compille le mot cl� `continue dans les boucles`
* Compile la d�claration de tableau sous la forme `int[] a;`
* Poss�de une pipeline de compilation permettant d'executer le code compil� directement
* Poss�de une gestion des logs formatt� avec assembleur et r�sultat d'�xecution (�volution pile ou erreurs)
* Poss�de une gestion des exceptions avanc�es (table de code d'erreurs avec param�tres injectables)
* Poss�de un linker basique (pour inclure un fichier du runtime via la forme `#include <NomFichier.h>`)


## Fonctionnalit�s non-fonctionnelles
* Le runtime print ne fonctionne pas, j'ignore compl�tement la raison. L'ex�cution de prends jamais fin pourtant j'ai v�rifi� mes boucles dans plusieurs cas dont celui utilis� par le print. Ma supposition est que la pile se rempli ce qui bloque le programme, j'ai donc v�rifi� mes dup et op�rations. Je n'ai pas r�ussi � trouver la cause de ce bug.

# Principales difficult�es
* La gestion des labels de boucles
* Le temps � allouer au projet (�tant donn� les autres enseignements et l'activit�e en entreprise)

# Exemple de code � compiler
Un set de tests est disponible sous le chemin `CompilerC--\test\` et r�parties en fonction de ce qu'ils testent