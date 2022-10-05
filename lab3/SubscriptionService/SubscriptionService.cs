using System.Collections;
using System.Data.SqlTypes;

namespace SubscriptionService;

public interface IBenefitService
{
				IEnumerable<string> GetBenefitsForCategory(Guid categoryGuid);
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

				//ежемесячная базовая стоимость
				private decimal _monthlyRate;

				//дополнительные опции подписки
				private List<Tuple<Guid, string>> _additionalOptions = new List<Tuple<Guid, string>>();

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
												return "Benefit for specifiend keyword wasn't foundn\n";
								}

								return appropriateBenefit;
				}

				DateTime GetBenefitExpireDateTime(Guid benefitGuid)
				{
								
								return _benefitService.GetBenefitExpireDateTime(benefitGuid);

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