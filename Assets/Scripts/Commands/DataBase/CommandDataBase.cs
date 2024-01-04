using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDataBase
{ 
    private Dictionary<string,Delegate> database = new Dictionary<string, Delegate>();

    public bool hasCommands(string commandName) => database.ContainsKey(commandName);

    public void AddCommand(string commandName, Delegate command)
    {
        if(!database.ContainsKey(commandName))
            database.Add(commandName, command);
        else
            Debug.LogError($"CommandDataBase: Command {commandName} already exists!");
    }

    public Delegate GetCommand(string commandName)
    {
        if(database.ContainsKey(commandName)){
            return database[commandName];
        }
        else{
            Debug.LogError($"CommandDataBase: Command {commandName} does not exist!");
            return null;
        }
    }
}
