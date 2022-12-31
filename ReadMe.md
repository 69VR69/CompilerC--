# Instructions de lancement
Le programme peut se lancer en ligne de commandes avec la commande suivant:
```
CompilerC__ "pathToFile"
```

Plusieurs arguments existent, et peuvent �tre combin�:
* `--simulator` : permet d'enchainer une execution du simulateur apr�s compilation
* `--test` : permet de lancer tous les tests (le chemin de fichier doit �tre �gale � `.`)
* `--debug` : permet de lancer le programme en mode debug (affiche les arbres de neuds, le code compil� ainsi que plusieurs informations de debug

Le programme peut �galement �tre lanc� depuis visual studio (vous pouvez configurer l'execution via les propri�t�s de debug)

Le resultat de la compilation est stock� au chemin `CompilerC--\bin\Debug\net6.0\assembly_files\nomDuFichier.c`.
Lorsque l'on execute avec le simulateur, un fichier de log est g�n�r� au chemin `CompilerC--\bin\Debug\net6.0\simulator\log\log_nomDuFichier.txt`

# Biblioth�ques utilis�es et autrees d�pendances
* [Cygwin.dll] : pour la compilation du projet


# Chemin de ficher et variables modifiables
* `CompilerC--\CompilerC--\Program.cs` : ligne 251
* `CompilerC--\CompilerC--\Util.cs` : sert de param�trage du compilateur