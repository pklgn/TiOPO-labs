using System.Data.SqlTypes;

namespace SubscriptionService;

public interface IBenefitService
{
				string GetBenefitDescription(Guid benefitGuid);
				DateTime GetBenefitExpireDateTime(Guid benefitGuid);
				IEnumerable<string> GetBenefitsForSubscriptionPlan(Guid subscriptionPlanGuid);
}

public interface IEmailSender
{
				public void SendEmail(string recipient, string subject, string body);
}

public class SubscriptionService
{
				private IBenefitService _benefitService;
				private IEmailSender _emailSender;

				//ежемесячная ставка
				private decimal _monthlyRate;

				private List<Tuple<Guid, string>> _additionalOptions = new List<Tuple<Guid, string>>();

				public SubscriptionService(IBenefitService benefitService, IEmailSender emailSender, decimal monthlyRate)
				{
								_benefitService = benefitService;
								_emailSender = emailSender;
								_monthlyRate = monthlyRate;
				}

				string GetBenefitDescription(Guid benefitGuid)
				{
				}

				DateTime GetBenefitExpireDateTime(Guid benefitGuid)
				{
				}

				IEnumerable<string> GetBenefitsForSubscriptionPlan(Guid subscriptionPlanGuid)
				{
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
								return _monthlyRate;
				}

				public Guid CreateAdditionOption(string name, string decription)
				{
				}

				public void RemoveAdditionOption(Guid additionalOptionGuid)
				{
				}
}