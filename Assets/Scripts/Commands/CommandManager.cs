using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
public class CommandManager : MonoBehaviour
{
    public static CommandManager instance {get; private set;}
    private CommandDataBase dataBase;
    // Start is called before the first frame update
    private void Awake() 
    {
        if(instance == null){
            instance = this;
            dataBase = new CommandDataBase();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] extentiontypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CommandDatabaseExtension))).ToArray();

            foreach (var extensions in extentiontypes)
            {
                var extendMethod = extensions.GetMethod("Extend");
                extendMethod.Invoke(null, new object[]{dataBase});
            }
        }
        else
            Destroy(this);
        
    }

    public void Execute(string commandName){
        Delegate command = dataBase.GetCommand(commandName);

        if(command != null)
            command.DynamicInvoke();
    }
}
