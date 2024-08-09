using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

/* options: 
    1. randomly generate the non conflicting numbers needed
       check if solveable
       good for lower numbers
    
    2. generate a couple of random numbers 
       check if solveable
       solve up to numbers needed
       good for higher numbers
    
    3. generate full board 
       remove numbers until good

    4. combo of 1&2
        1 up to a certain number
        2 higher than that number

problems:
    can't verify any option as effecient without using it
    subcomponent sudoku solver(verifys if a puzzle is solveable) would need a guessing component for 2 and 4
    or else a compatabillty with a guessing component

plan:

    make a solver
    make 1.
    test
    if too slow make 2.
*/

// the board is laid out as a 9x9 2d array
// [0,0] relates to the bottom left corner 
// first array is x, second is y
// 0 is for blank spaces

namespace Sudoko_Creater
{

    class Program
    {

        static int[,] random_board(int target_nums)
        {
        Random rand = new Random();

            if(target_nums > 30)
            {
            //too slow to randomly generate
            int[,] small_board = random_board(23);
            int[,] big_board = sudoku_solver(small_board,81-target_nums);
            
            //the solver can add mutiple numbers at once
            int zeros = 0;
            for(int i = 0;i < 9; i++)
            {
                for(int i2 = 0; i2< 9;i2++)
                {
                    if(big_board[i,i2] == 0)
                    {
                        zeros++;
                    }
                }
            }
            if(zeros == 81 - target_nums)
            {
                return big_board;
            }
            else
            {
                while(zeros < 81 - target_nums)
                {
                    int x = rand.Next(0,9);
                    int y = rand.Next(0,9);
                    if(big_board[x,y] != 0)
                    {
                        big_board[x,y] = 0;
                        zeros++;
                    }
                }
                int[,] temp_board = new int[9,9];
                for(int i = 0;i < 9; i++)
                {
                    for(int i2 = 0; i2 < 9; i2++)
                    {
                        temp_board[i,i2] = big_board[i,i2];
                    }
                }
                if(is_solved(sudoku_solver(temp_board)))
                {
                    return big_board;
                }
                else
                {
                    return random_board(target_nums);
                }
            }

            }
            int[,] board = new int[9,9];
            int number_of_nums = 0;
            while(number_of_nums < target_nums)
            {
                int type = rand.Next(0,2);
                string mode = "";
                if(type == 0)
                {
                    mode = "column";
                }
                else if(type == 1)
                {
                    mode = "row";
                }
                int num = rand.Next(1,10);
                int xy = rand.Next(0,9);
                int[] spots = possible_line_spots(board,num,xy,mode);
                if(spots.Length < 2)
                {
                    continue;
                }
                int spot = spots[rand.Next(0,spots.Length-1)];
                if(mode == "column")
                {
                    board[xy,spot] = num;
                    number_of_nums++;
                }
                else
                {
                    board[spot,xy] = num;
                    number_of_nums++;
                }
            }
            int[,] solving_board = new int[9,9];
            for(int i = 0;i<9;i++)
            {
                for(int i2 = 0;i2 < 9;i2++)
                {
                    solving_board[i,i2] = board[i,i2];
                }
            }
            if(is_solved(sudoku_solver(solving_board)))
            {
                return board;
            }
            else
            {
                return random_board(target_nums);
            }
        }
        static int[] possible_line_spots(int[,] board, int num, int xy, string mode)
        {
            
            int[] coordinates = [];
            if(mode.ToLower() == "row")
            {
                //checking theres no num already in the line
                for(int i =0; i<9;i++)
                {
                    if(board[i,xy] == num)
                    {
                        return [];
                    }
                }

                for(int i =0; i<9;i++)
                {
                    // checking theres not another num there
                    if(board[i,xy] != 0)
                    {
                        continue;
                    }
                    if(spot_checker(board,i,xy,num,mode))
                    {
                        int[] temp_coordinates = new int[coordinates.Length];
                        int i3 = 0;
                        foreach(int i2 in coordinates)
                        {
                            temp_coordinates[i3] = i2;
                            i3++;
                        }

                        coordinates = new int[coordinates.Length+1];
                        i3 = 0;
                        foreach(int i2 in temp_coordinates)
                        {
                            coordinates[i3] = i2;
                            i3++;
                        }

                        coordinates[coordinates.Length-1] = i; 
                    }
                }
            }

            else if(mode.ToLower() == "column")
            {
                for(int i = 0; i <9;i++)
                {
                    if(board[xy,i] ==num)
                    {
                        return [];
                    }
                }

                for(int i = 0;i <9;i++)
                {
                    if(board[xy,i] != 0)
                    {
                        continue;
                    }
                    
                    if(spot_checker(board,xy,i,num,mode))
                    {
                        int[] temp_coordinates = new int[coordinates.Length];
                        int i3 = 0;
                        foreach(int i2 in coordinates)
                        {
                            temp_coordinates[i3] = i2;
                            i3++;
                        }

                        coordinates = new int[coordinates.Length+1];
                        i3 = 0;
                        foreach(int i2 in temp_coordinates)
                        {
                            coordinates[i3] = i2;
                            i3++;
                        }

                        coordinates[coordinates.Length-1] = i; 
                    }
                }
            }


            return coordinates;
        }
        static bool spot_checker(int[,] board, int x, int y, int num, string mode = "none")
        {
            //column
            if(mode.ToLower() != "column")
            {
                for(int i =0; i < 9; i++)
                {
                    if(i == y)
                    {
                        continue;
                    }
                    if(board[x,i] == num)
                    {
                        return false;
                    }
                }
            }
            //row
            if(mode.ToLower() != "row")
            {
                for(int i =0; i< 9; i++)
                {
                    if(i==x)
                    {
                        continue;
                    }
                    if(board[i,y]==num)
                    {
                        return false;
                    }
                }
            }
            //box
            if(mode.ToLower() != "box")
            {
                int box_starting_x = x/3*3;
                int box_starting_y = y/3*3;
                for(int i = box_starting_x; i < box_starting_x+3; i++)
                {
                    if(i==x)
                    {
                        continue;
                    }
                    for(int i2 = box_starting_y; i2 < box_starting_y+3; i2++)
                    {
                        if(i2==y)
                        {
                            continue;
                        }
                        if(board[i,i2] == num)
                        {
                            return false;
                        }
                    }
                }
            } 

            return true;
        }

        static int[] possible_box_spots(int [,] board, int num, int box)
        {
            //boxes are labeled 1-9 where box num mod 3 equals the x and box num div 3 equals the y
            // coordinates in each box use the same system

            int x = (((box - 1) % 3)) *3;
            int y = ((box -1) / 3) *3;

            int[] coordinates = [];

            //checks theres no other of the num in the box
            for(int i = x;i < x+3;i++)
            {
                for(int i2 = y; i2 < y+3; i2++)
                {
                    if(board[i,i2]==num)
                    {
                        return [];
                    }
                }
            }

            for(int i = x;i < x+3;i++)
            {
                for(int i2 = y; i2 < y+3; i2++)
                {
                    if(board[i,i2] != 0)
                    {
                        continue;
                    }
                    if(spot_checker(board,i,i2,num,"box"))
                    {
                        int[] temp_coordinates = new int[coordinates.Length];
                        int i4 = 0;
                        foreach(int i3 in coordinates)
                        {
                            temp_coordinates[i4] = i3;
                            i4++;
                        }

                        coordinates = new int[coordinates.Length+2];
                        i4 = 0;
                        foreach(int i3 in temp_coordinates)
                        {
                            coordinates[i4] = i2;
                            i4++;
                        }

                        coordinates[coordinates.Length-2] = i;
                        coordinates[coordinates.Length-1] = i2;

                    }
                }
            }


            return coordinates;
        }


        static int[,] sudoku_solver(int[,] board,int completness = 0)
        {
            int zeros = 0;
            for(int i = 0;i < 9;i++)
            {
                for(int i2 = 0;i2 < 9; i2++)
                {
                    if(board[i,i2] == 0)
                    {
                        zeros++;
                    }
                }
            }
            if(zeros < completness)
            {
                return board;
            }
        /* solving steps:
            1. check each numbers column, row and box and fill in any that have only 1 spot
            repeat until stuck
            2. when stuck, check all open squares' numbers if any square has only 1 number fill it
            repeat 1 and 2 until stuck
            3. when stuck again, check if any number box spots are all on 1 coloumn or row, if so remove any
            other spots in that row or column
        */

        // iterate through nums 1-9 and each num's boxes, rows and columns
        bool num_added = false;

        for(int i = 1; i < 10; i++)
        {
            //iterating through columns
            for(int i2 = 0; i2 < 9; i2++)
            {
                int[] coordinates = possible_line_spots(board,i,i2,"column");
                if(coordinates.Length == 1)
                {
                    board[i2,coordinates[0]] = i;
                    num_added = true;
                }
            }
            //iterating through rows
            for(int i2 = 0; i2 < 9; i2++)
            {
                int[] coordinates = possible_line_spots(board,i,i2,"row");
                if(coordinates.Length == 1)
                {
                    board[coordinates[0],i2] = i;
                    num_added = true;
                }
            }
            //iterating through boxes
            for(int i2 = 1; i2 < 10; i2++)
            {
                int[] coordinates = possible_box_spots(board,i,i2);
                if(coordinates.Length == 2)
                {
                    board[coordinates[0],coordinates[1]] = i;
                    num_added = true;
                }
            }
        }
        if(num_added)
        {
            return sudoku_solver(board);
        }
        // step 1 has failed at this point so the same thing is done with step 2 & 3

        for(int i = 0; i < 9;i++)
        {
            for(int i2 = 0; i2 < 9; i2++)
            {
                if(board[i,i2] != 0)
                {
                    continue;
                }

                int[] possible_nums = [];

                for(int i3 = 1; i3 < 10;i3++)
                {
                    if(spot_checker(board,i,i2,i3))
                    {
                        int[] temp_possible_nums = new int[possible_nums.Length];
                        int i5 = 0;
                        foreach(int i4 in possible_nums)
                        {
                            temp_possible_nums[i5] = i4;
                            i5++;
                        }
                        possible_nums = new int[possible_nums.Length+1];

                        i5 = 0;
                        foreach(int i4 in temp_possible_nums)
                        {
                            possible_nums[i5] = i4;
                        }

                        possible_nums[temp_possible_nums.Length] = i3;
                    }
                }

                if(possible_nums.Length == 1)
                {
                    board[i,i2] = possible_nums[0];
                    num_added = true;
                }
            }
        }

        if(num_added)
        {
            return sudoku_solver(board);
        }
        // step 3
        //check each box
        //check if all spots on one row or columun
        // if so check all affected rows/columns/boxes
        for(int i = 0; i < 9;i++)
        {
            for(int i2 = 1; i2 < 10;i2++)
            {
                int[] spots = possible_box_spots(board,i,i2);
                // here I have to assign values or vs code shouts at me
                string mode = "";
                int xy = 0;
                // here you double the number of spots because x and y are seperate
                if(spots.Length == 4)
                {
                    if(spots[0] == spots[2])
                    {
                        mode = "column";
                        xy = spots[0];
                    }
                    if(spots[1] == spots[3])
                    {
                        mode = "row";
                        xy = spots[1];
                    }
                }
                else if(spots.Length == 6)
                {
                    if(spots[0] == spots[2] && spots[2] == spots[4])
                    {
                        mode = "column";
                        xy = spots[0];
                    }
                    if(spots[1] == spots[3] && spots[3] == spots[5])
                    {
                        mode = "row";
                        xy = spots[1];
                    }
                }
                else
                {
                    continue;
                }

                int[,] new_board = new int[9,9];
                    for(int i4 = 0; i4 < 9;i4++)
                    {
                        for(int i5 = 0;i5 < 9;i5++)
                        {
                            new_board[i4,i5] = board[i4,i5];
                        }
                    } 
                //here only executes if the box spots were on one line
                int[] line_spots = possible_line_spots(board,i,xy,mode);
                string opposite_mode;
                if(line_spots.Length * 2 > spots.Length)
                {
                    foreach(int i3 in line_spots)
                    {
                        // here im doing this for ease
                        if(mode == "column")
                        {
                            new_board[xy,i3] = 10;
                            opposite_mode = "row";
                        }
                        else
                        {
                            new_board[i3,xy] = 10;
                            opposite_mode = "column";
                        }
                        
                        bool in_box = false;
                        foreach(int i4 in spots)
                        {
                            if(i3 == i4)
                            {
                                in_box = true;
                                break;
                            }
                        }
                        if(!in_box)
                        {
                            int[] affected_line_spots = possible_line_spots(new_board,i,i3,opposite_mode);
                            int x;
                            int y;
                            if(mode == "column")
                            {
                                x = xy;
                                y = i3;
                            }
                            else
                            {
                                x = i3;
                                y = xy;
                            }
                            if(affected_line_spots.Length == 1)
                            {
                                board[x,y] = i;
                                num_added = true;
                                continue;

                            }
                            int[] affected_box_spots = possible_box_spots(new_board,i,((x % 3)+y/3 *3)+1);
                            if(affected_box_spots.Length == 2)
                            {
                                board[x,y] = i;
                                num_added = true;
                                continue;
                            }
                        }

                    }
                }
            }
        }
        if(num_added)
        {
            return sudoku_solver(board);
        }

            return board;
        }
        
        static bool is_solved(int[,] board)
        {

            for(int i = 0;i<9;i++)
            {
                for(int i2 = 0;i2<9;i2++)
                {
                    Console.Write(board[i2,8-i]+" ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
            bool solved = true;
            for(int i = 0;i < 9;i++)
            {
                for(int i2 = 0; i2 < 9;i2++)
                {
                    if(board[i,i2] == 0)
                    {
                        solved = false;
                    }
                }
            }
            if(solved)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        static void Main(string[] args)
        {
            //here i'm painfully inputting a full board to test the solver
            /*int[,] example_board = new int[9,9];
            example_board[0,0] = 6;
            example_board[0,1] = 9;
            example_board[0,3] = 7;
            example_board[0,5] = 8;
            example_board[0,7] = 2;
            example_board[0,8] = 3;

            example_board[1,4] = 6;

            example_board[2,1] = 4;
            example_board[2,3] = 2;
            example_board[2,5] = 9;
            example_board[2,7] = 1;

            example_board[3,0] = 1;
            example_board[3,2] = 5;
            example_board[3,6] = 2;
            example_board[3,8] = 8;

            example_board[4,1] = 8;
            example_board[4,7] = 3;

            example_board[5,0] = 7;
            example_board[5,2] = 9;
            example_board[5,6] = 4;
            example_board[5,8] = 1;

            example_board[6,1] = 7;
            example_board[6,3] = 4;
            example_board[6,5] = 1;
            example_board[6,7] = 6;

            example_board[7,4] = 5;

            example_board[8,0] = 3;
            example_board[8,1] = 5;
            example_board[8,3] = 9;
            example_board[8,5] = 6;
            example_board[8,7] = 4;
            example_board[8,8] = 2;

            if(sudoku_solver(example_board))
            {
                Console.WriteLine("Solveable");
            }
            else
            {
                Console.WriteLine("Unsolveable");
            }
            */
            int target_nums = 0;
            bool not_valid = true;
            while(not_valid)
            {
                Console.WriteLine("How many numbers do you want? 17-80, 25 is optimal");
                not_valid = false;
                try
                {
                    target_nums = Convert.ToInt32(Console.ReadLine());
                    if(target_nums < 17 || target_nums > 80)
                    {
                        not_valid = true;
                        Console.WriteLine("Number is out of range");
                    }
                }
                catch
                {
                    not_valid = true;
                    Console.WriteLine("Incorrect type");
                }
            }
            int[,] board = random_board(target_nums);
            for(int i = 0;i<9;i++)
            {
                for(int i2 = 0;i2<9;i2++)
                {
                    Console.Write(board[i2,8-i]+" ");
                }
                Console.WriteLine("");
            }
        }
    }
}