# Sudoku Solver Windows

This is a sudoku solver on windows, which solves by using a backtracking algorithm. I used this project to get more into algorithms, Enjoy!  
  
A Website version might come later but still is not in development. This is one of my first projects so take it with a grain of salt and always remember feedback is really appreciated :)  
  
## More about the App

It is made with Visual Studio 2022 in C# using WPF  

<img src="https://github.com/UnnamedH/SudokuSolver/assets/47675080/f1a8e435-c58c-47d7-ba5a-8640f9329706" width=600>

  
### The board
It has a default sudoku board that loads when the app launches, you can import and export boards as text files.  
The format is really simple its a series of numbers that the app reads, 0 s as blanks and any number as the number at its respective position on the board. And writes the numbers row by row.

  
### The modes
The app has 2 solving modes "Normal Mode", enabled by default, which solves the puzzle by also showcasing each step the backtracking algorithm takes to solve the problem.  
The "Fast Mode" is just a mode that instantly solves the puzzle.
