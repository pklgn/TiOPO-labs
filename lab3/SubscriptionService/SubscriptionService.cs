// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace SubscriptionService;

using AdditionalOption = Tuple<Guid, string>;

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

    //ежемесячная базовая стоимость
    private decimal _monthlyRate;

    //дополнительные опции подписки
    private List<AdditionalOption> _additionalOptions = new List<Tuple<Guid, string>>();

    public SubscriptionService(IBenefitService benefitService, IEmailSender emailSender, decimal monthlyRate)
    {
        _benefitService = benefitService;
        _emailSender = emailSender;
        _monthlyRate = monthlyRate;
    }

    string GetBenefitFromCategory(string keyword, Guid categoryId)
    {
        var benefits = _benefitService.GetBenefitsForCategory(categoryId);
        var appropriateBenefit = benefits.Where(benefit => benefit.ToLower().Contains(keyword)).FirstOrDefault();

        if (appropriateBenefit == null)
        {
            return "Benefit for specifiend keyword wasn't found\n";
        }

        return appropriateBenefit;
    }

    DateTime GetBenefitExpireDateTime(Guid benefitGuid)
    {
        return _benefitService.GetBenefitExpireDateTime(benefitGuid);
    }

    public void SendEmail(string recipient, string notification)
    {
        string subject = "Subscription service updates";
        string body = $"Hi,\n{notification}.\nSincerely, the service team.";

        _emailSender.SendEmail(recipient, subject, body);
    }

    public decimal CalculatePriceForPeriod(DateTime periodStart, DateTime periodEnd)
    {
        //TODO: логика того, что подписка не может быть меньше чем на месяц
        if (periodStart > periodEnd)
        {
            throw new ArgumentException($"Start period {periodStart} must precede end period value {periodEnd}");
        }

        var periodInMonths = GetDiffInMonths(periodStart, periodEnd);

        if (periodInMonths == 0)
        {
            throw new Exception("The difference between the periods cannot be less than one month");
        }

        return _monthlyRate * periodInMonths;
    }

    public Guid CreateAdditionOption(string name, string decription)
    {
        Guid guid = Guid.NewGuid();
        string additionalOptionString = $"{name}: {decription}";
        _additionalOptions.Add(new AdditionalOption(guid, additionalOptionString));

        return guid;
    }

    public void RemoveAdditionOption(Guid additionalOptionGuid)
    {
        _additionalOptions.Where(option => option.Item1 == additionalOptionGuid).FirstOrDefault();
    }

    public IEnumerable<AdditionalOption> GetAdditionalOptions()
    {
        return _additionalOptions;
    }

    private static Int32 GetDiffInMonths(DateTime start, DateTime end)
    {
        Int32 months = 0;
        DateTime tmp = start;

        while (tmp < end)
        {
            months++;
            tmp = tmp.AddMonths(1);
        }

        return months;
    }
}
