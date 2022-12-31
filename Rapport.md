# Liste des fonctionnalités du compilateur
## Fonctionnalités essentielles
* Compile les expressions
* Compile les conditionelles
* Compile les variables
* Compile les boucles
* Compile les fonctions
* Compile les pointeurs et tableaux
* Possède un runtime pour les fonction `print()`, `malloc()` et `free()`

## Fonctionnalités supplémentaires
* Compille le mot clé `continue dans les boucles`
* Compile la déclaration de tableau sous la forme `int[] a;`
* Possède une pipeline de compilation permettant d'executer le code compilé directement
* Possède une gestion des logs formatté avec assembleur et résultat d'éxecution (évolution pile ou erreurs)
* Possède une gestion des exceptions avancées (table de code d'erreurs avec paramètres injectables)
* Possède un linker basique (pour inclure un fichier du runtime via la forme `#include <NomFichier.h>`)


## Fonctionnalités non-fonctionnelles
* Le runtime print ne fonctionne pas, j'ignore complètement la raison. L'exécution de prends jamais fin pourtant j'ai vérifié mes boucles dans plusieurs cas dont celui utilisé par le print. Ma supposition est que la pile se rempli ce qui bloque le programme, j'ai donc vérifié mes dup et opérations. Je n'ai pas réussi à trouver la cause de ce bug.

# Principales difficultées
* La gestion des labels de boucles
* Le temps à allouer au projet (étant donné les autres enseignements et l'activitée en entreprise)

# Exemple de code à compiler
Un set de tests est disponible sous le chemin `CompilerC--\test\` et réparties en fonction de ce qu'ils testent