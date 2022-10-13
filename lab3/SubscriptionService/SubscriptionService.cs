// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Subscription;

using AdditionalOptionWithCost = Tuple<Guid, string, decimal>;

public interface IBenefitService
{
    IEnumerable<string> GetBenefitsForCategory(Guid categoryGuid);
    DateTime GetBenefitExpireDateTime(Guid benefitGuid);
}

public interface IEmailSender
{
    public void SendEmail(string recipient, string subject, string body);
}

public class SubscriptionService
{
    private IBenefitService _benefitService;
    private IEmailSender _emailSender;

    private const decimal _standartCost = 100;

    //ежемесячная стоимость
    private decimal _monthlyCost;

    //дополнительные опции подписки
    private List<AdditionalOptionWithCost> _additionalOptions = new List<Tuple<Guid, string, decimal>>();

    public SubscriptionService(IBenefitService benefitService, IEmailSender emailSender, decimal monthlyRate = _standartCost)
    {
        _benefitService = benefitService;
        _emailSender = emailSender;
        _monthlyCost = monthlyRate;
    }

    public string SearchBenefitFromCategory(string keyword, Guid categoryId)
    {
        var benefits = _benefitService.GetBenefitsForCategory(categoryId);
        var appropriateBenefit = benefits.Where(benefit => benefit.ToLower().Contains(keyword)).FirstOrDefault();

        if (appropriateBenefit == null)
        {
            return "";
        }

        return appropriateBenefit;
    }

    public DateTime GetBenefitExpireDateTime(Guid benefitGuid)
    {
        return _benefitService.GetBenefitExpireDateTime(benefitGuid);
    }

    public IEnumerable<string> GetAllBenefitsForCategory(Guid categoryId)
    {
        return _benefitService.GetBenefitsForCategory(categoryId);
    }

    public void SendEmail(string recipient, string notification)
    {
        string subject = "Subscription service updates";
        string body = $"Hi,\n{notification}.\nSincerely, the service team.";

        _emailSender.SendEmail(recipient, subject, body);
    }

    public decimal CalculateDateTimeRangeCost(DateTime rangeBegin, DateTime rangeEnd)
    {
        //FIXED: логика того, что подписка не может быть меньше чем на месяц
        if (rangeBegin > rangeEnd)
        {
            throw new ArgumentException("Begin range value must precede end range value");
        }

        var rangeInMonths = GetDiffInMonths(rangeBegin, rangeEnd);

        if (rangeInMonths == 0)
        {
            throw new Exception("The difference between the periods cannot be less than one month");
        }

        return _monthlyCost * rangeInMonths;
    }

    public Guid CreateAdditionalOption(string name, string decription, decimal monthlyCost)
    {
        Guid guid = Guid.NewGuid();
        string additionalOptionString = $"{name}: {decription}";
        _additionalOptions.Add(new AdditionalOptionWithCost(guid, additionalOptionString, monthlyCost));

        return guid;
    }

    public void RemoveAdditionalOption(Guid additionalOptionGuid)
    {
        var optionToRemove = _additionalOptions.Where(option => option.Item1 == additionalOptionGuid).FirstOrDefault();
        if (optionToRemove != null)
        {
            _additionalOptions.Remove(optionToRemove);
        }
    }

    public IEnumerable<AdditionalOptionWithCost> GetAdditionalOptions()
    {
        return _additionalOptions;
    }

    private static Int32 GetDiffInMonths(DateTime start, DateTime end)
    {
        Int32 months = 0;
        DateTime tmp = start;

        if (start.AddMonths(1) > end)
        {
            return months;
        }

        while (tmp < end)
        {
            months++;
            tmp = tmp.AddMonths(1);
        }

        return months;
    }
}
