using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_DatabaseExtension_Example : CommandDatabaseExtension
{
    new public static void Extend(CommandDataBase database)
    {
        database.AddCommand("print", new Action(PrintDefaultMessage));
    }

    private static void PrintDefaultMessage()
    {
        Debug.Log("This is a default message");
    }   
}