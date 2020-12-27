using System.Collections.Generic;
using System;


class Program
{
    static void Main(string[] args)
    {
        Test test_1 = new Test(new string[] { "белый", "синий" });
        Test test_2 = new Test(new string[] { "белый", "синий" });

        Test[] arr = new Test[2];
        arr[0] = test_1;
        arr[1] = test_2;

        if (arr[0] == test_1) Console.WriteLine("Yes");
        else Console.WriteLine("No");

        if (test_1 == test_2) Console.WriteLine("Yes");
        else Console.WriteLine("No");


        Test.FS(ref test_1, ref test_2);
        Console.WriteLine(test_1);
        Console.WriteLine(test_2);

        Console.WriteLine("======");

        TestList testList = new TestList();
        testList.equalEvent += TestList_equalEvent;
        foreach (var item in testList.StringEnumerator()) Console.WriteLine(item);
    }

    private static void TestList_equalEvent(object source, string args)
    {
        Console.WriteLine($" args: {args.ToString()}");
    }
}

class Test
{
    string[] words { get; set; }
    public Test(string[] words)
    {
        this.words = words;
    }
    public string Word1
    {
        get { return words[0]; }
        set { words[0] = value; }
    }

    public string Word2
    {
        get { return words[1]; }
        set { words[1] = value; }
    }

    public static void FS(ref Test test1, ref Test test2)
    {
        test1.Word1 = "white";
        test1 = new Test(new string[] { "red", "green" });
        test2 = new Test(new string[] { "red", "green" });
        test2.Word2 = "blue";
    }
    public override string ToString()
    {
        return $"{words[0]} {words[1]}";
    }

}

delegate void TestHandler(object source, string args);

class TestList
{
    public List<Test> tList { get; private set; }
    public event TestHandler equalEvent;

    public TestList()
    {
        tList = new List<Test>();
        tList.Add(new Test(new string[] { "red", "red" }));
        tList.Add(new Test(new string[] { "white", "black" }));
        tList.Add(new Test(new string[] { "green", "green" }));
    }

    public IEnumerable<string> StringEnumerator()
    {
        foreach (Test item in tList)
        {
            if (item.Word1 == item.Word2)
            {
                if (equalEvent != null) equalEvent(this, item.Word1);
                yield return item.Word1;
            }
            else yield return item.ToString();
        }

    }
}