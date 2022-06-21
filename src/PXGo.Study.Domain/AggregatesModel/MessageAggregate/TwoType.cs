using PXGo.Study.Domain.SeedWork;

namespace PXGo.Study.Domain.AggregatesModel.MessageAggregate;

public class TwoType : Enumeration
{
    public static TwoType Default = new(0, "預設");
    public static TwoType One = new(1, "1");
    public static TwoType Two = new(2, "2");

    public TwoType(int id, string name) : base(id, name)
    {
    }

    public static IEnumerable<TwoType> List()
    {
        return new[]
        {
            Default, One, Two
        };
    }

    public static TwoType FromName(string name)
    {
        var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (state == null) throw new Exception($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
        return state;
    }

    public static TwoType From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);
        if (state == null)
        {
            var idNameList = string.Join(",", List().Select(s => s.Name));
            throw new Exception($"Possible values for OrderStatus: {idNameList}, But assign {id}");
        }
        return state;
    }
}
