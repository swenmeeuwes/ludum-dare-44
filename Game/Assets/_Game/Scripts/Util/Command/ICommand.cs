public interface ICommand<in TParam1>
{
    void Execute(TParam1 param1);
}

public interface ICommand<in TParam1, in TParam2>
{
    void Execute(TParam1 param1, TParam2 param2);
}

public interface ICommand<in TParam1, in TParam2, in TParam3>
{
    void Execute(TParam1 param1, TParam2 param2, TParam3 param3);
}