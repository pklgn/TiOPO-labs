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

    private decimal _monthlyCost;

    private List<AdditionalOptionWithCost> _additionalOptions = new List<Tuple<Guid, string, decimal>>();

    /// <summary>
    /// Create subscription service which allows you work with all it benefits and options.
    /// </summary>
    public SubscriptionService(IBenefitService benefitService, IEmailSender emailSender, decimal monthlyRate = _standartCost)
    {
        _benefitService = benefitService;
        _emailSender = emailSender;
        _monthlyCost = monthlyRate;
    }

    /// <summary>
    /// Allows you to get exactly one benefit found by your keyword
    /// </summary>
    /// <returns>
    /// String that represent first found value.
    /// If nothing found returns an empty string.
    /// </returns>
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

    /// <summary>
    /// Allows you to get datetime when specified benefit will expire.
    /// </summary>
    /// <returns>
    /// Datetime of the benefit expiration.
    /// </returns>
    public DateTime GetBenefitExpireDateTime(Guid benefitGuid)
    {
        return _benefitService.GetBenefitExpireDateTime(benefitGuid);
    }

    /// <summary>
    /// Allows you to get all available benefits for the specified category
    /// </summary>
    /// <returns>
    /// <see cref="IEnumerable<string>"/>
    /// </returns>
    public IEnumerable<string> GetAllBenefitsForCategory(Guid categoryId)
    {
        return _benefitService.GetBenefitsForCategory(categoryId);
    }

    /// <summary>
    /// Create default service subject for the email and paste specified notification into the email body.
    /// </summary>
    public void SendEmail(string recipient, string notification)
    {
        string subject = "Subscription service updates";
        string body = $"Hi,\n{notification}.\nSincerely, the service team.";

        _emailSender.SendEmail(recipient, subject, body);
    }

    /// <summary>
    /// Allows you to get subscription cost based on your monthly cost for a specified datetime range.
    /// </summary>
    /// <returns>
    /// Calculated cost for appropriate datetime range.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If invalid range order given.
    /// Or if the range is less than one month.
    /// </exception>
    public decimal CalculateDateTimeRangeCost(DateTime rangeBegin, DateTime rangeEnd)
    {
        if (rangeBegin > rangeEnd)
        {
            throw new ArgumentException("Begin range value must precede end range value");
        }

        var rangeInMonths = GetDiffInMonths(rangeBegin, rangeEnd);

        if (rangeInMonths == 0)
        {
            throw new ArgumentException("The difference between the periods cannot be less than one month");
        }

        return _monthlyCost * rangeInMonths;
    }

    /// <summary>
    /// Allows you to add new additional option and save it.
    /// </summary>
    /// <returns>
    /// Guid of the newly created additional option.
    /// </returns>
    public Guid CreateAdditionalOption(string name, string decription, decimal monthlyCost)
    {
        Guid guid = Guid.NewGuid();
        string additionalOptionString = $"{name}: {decription}";
        _additionalOptions.Add(new AdditionalOptionWithCost(guid, additionalOptionString, monthlyCost));

        return guid;
    }

    /// <summary>
    /// Try to remove additional option by specified guid value.
    /// If nothing found does nothing.
    /// </summary>
    public void RemoveAdditionalOption(Guid additionalOptionGuid)
    {
        var optionToRemove = _additionalOptions.Where(option => option.Item1 == additionalOptionGuid).FirstOrDefault();
        if (optionToRemove != null)
        {
            _additionalOptions.Remove(optionToRemove);
        }
    }

    /// <summary>
    /// Allow you to get all saved additional options.
    /// </summary>
    /// <returns>
    /// <see cref="IEnumerable{AdditionalOptionWithCost}"/>.
    /// By default it is empty.
    /// </returns>
    public IEnumerable<AdditionalOptionWithCost> GetAdditionalOptions()
    {
        return _additionalOptions;
    }

    /// <summary>
    /// Allows you to get mothly cost value.
    /// </summary>
    /// <returns>
    /// <see cref="IEnumerable{AdditionalOptionWithCost}"/>
    /// </returns>
    public decimal GetMonthlyCost()
    {
        return _monthlyCost;
    }

    /// <summary>
    /// Set a new mothly cost value.
    /// </summary>
    public void SetMonthlyCost(decimal cost)
    {
        _monthlyCost = cost;
    }

    /// <summary>
    /// Calculate difference between two datetime periods in months with strict order.
    /// </summary>
    /// <returns>
    /// Calculated difference.
    /// </returns>
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
