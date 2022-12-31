# Instructions de lancement
Le programme peut se lancer en ligne de commandes avec la commande suivant:
```
CompilerC__ "pathToFile"
```

Plusieurs arguments existent, et peuvent être combiné:
* `--simulator` : permet d'enchainer une execution du simulateur après compilation
* `--test` : permet de lancer tous les tests (le chemin de fichier doit être égale à `.`)
* `--debug` : permet de lancer le programme en mode debug (affiche les arbres de neuds, le code compilé ainsi que plusieurs informations de debug

Le programme peut également être lancé depuis visual studio (vous pouvez configurer l'execution via les propriétés de debug)

Le resultat de la compilation est stocké au chemin `CompilerC--\bin\Debug\net6.0\assembly_files\nomDuFichier.c`.
Lorsque l'on execute avec le simulateur, un fichier de log est généré au chemin `CompilerC--\bin\Debug\net6.0\simulator\log\log_nomDuFichier.txt`

# Bibliothèques utilisées et autrees dépendances
* [Cygwin.dll] : pour la compilation du projet


# Chemin de ficher et variables modifiables
* `CompilerC--\CompilerC--\Program.cs` : ligne 251
* `CompilerC--\CompilerC--\Util.cs` : sert de paramétrage du compilateur