using PXGo.Study.Domain.AggregatesModel.Enums;
using PXGo.Study.Domain.SeedWork;

namespace PXGo.Study.Domain.AggregatesModel.MessageAggregate;

public class MessageEntity : Entity, IAggregateRoot
{

    public OneType TypeOne { get; private set; }

    public TwoType TypeTwo { get; private set; }

    public string Content { get; private set; }

    public DateTime CreateTime { get; private set; }


    public MessageEntity() { }

    public MessageEntity(OneType typeOne, int typeTwo, string content)
    {
        TypeOne = typeOne;
        TypeTwo = TwoType.From(typeTwo);
        Content = content;
        CreateTime = DateTime.Now;
    }

    public void UpdateMessage(OneType typeOne, int typeTwo, string content)
    {
        TypeOne = typeOne;
        TypeTwo = TwoType.From(typeTwo);
        Content = content;      
    }

}
