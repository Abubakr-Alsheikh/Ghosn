using Microsoft.AspNetCore.Mvc;
using Ghosn_BLL;

namespace Ghosn.Controllers;

[ApiController]
[Route("api/Ghosn")]
public class GhosnController : ControllerBase
{
    // GET: api/Clients
    [HttpGet("Clients")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ClientDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<ClientDTO>> GetAllClients()
    {
        var clients = clsClients_BAL.GetAllClients();

        if (clients == null || clients.Count == 0)
        {
            return NotFound("No clients found.");
        }

        return Ok(clients);
    }

    // GET: api/Clients/5
    [HttpGet("Client/{ClientID}", Name = "GetClientById")] // Ensure the Name matches
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ClientDTO> GetClientById(int ClientID)
    {
        var client = clsClients_BAL.GetClientById(ClientID);

        if (client == null)
        {
            return NotFound($"Client with ID {ClientID} not found.");
        }

        return Ok(client);
    }

    // POST: api/Clients
    [HttpPost("Client")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClientDTO))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ClientDTO> AddClient([FromBody] ClientDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Invalid client data.");
        }

        try
        {
            int clientId = clsClients_BAL.AddClient(dto);
            dto.ClientID = clientId; // Set the ClientID in the DTO
            return CreatedAtRoute("GetClientById", new { ClientID = clientId }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Clients/5
    [HttpPut("Client/{ClientID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateClient(int ClientID, [FromBody] ClientDTO dto)
    {
        if (dto == null || ClientID < 1)
        {
            return BadRequest("Invalid client data or ID mismatch.");
        }

        try
        {
            dto.ClientID = ClientID;

            bool isUpdated = clsClients_BAL.UpdateClient(dto);

            if (!isUpdated)
            {
                return NotFound($"Client with ID {ClientID} not found.");
            }

            return Ok(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE: api/Clients/5
    [HttpDelete("Client/{ClientID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteClient(int ClientID)
    {
        if (ClientID <= 0)
        {
            return BadRequest("Invalid Client ID.");
        }

        try
        {
            bool isDeleted = clsClients_BAL.DeleteClient(ClientID);

            if (!isDeleted)
            {
                return NotFound($"Client with ID {ClientID} not found.");
            }

            return Ok($"Client with ID {ClientID} has been deleted.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("AllPlans", Name = "GetAllPlansWithDetails")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<PlanResponseDTO>> GetAllPlansWithDetails()
    {
        var plans = clsPlans_BLL.GetAllPlansWithDetails();
        return plans.Count > 0 ? Ok(plans) : NotFound("No plans found.");
    }

    // Retrieve a Plan by ID with related data
    [HttpGet("Plan/PlanID/{PlanID}", Name = "GetPlanWithDetailsById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PlanResponseDTO> GetPlanWithDetailsById(int PlanID)
    {
        if (PlanID <= 0)
            return BadRequest($"ID {PlanID} is not valid.");

        var plan = clsPlans_BLL.GetPlanWithDetailsById(PlanID);
        return plan != null ? Ok(plan) : NotFound($"Plan with ID {PlanID} not found.");
    }

    // Add a new Plan with related data
    [HttpPost("Plan/{ClientID}", Name = "AddPlanWithDetails")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PlanResponseDTO> AddPlanWithDetails(int ClientID, [FromBody] PlanResponseDTO newPlan)
    {
        if (newPlan == null || newPlan.Output == null || newPlan.Input == null)
            return BadRequest("Plan data, Output, and Input are required.");

        newPlan.ClientID = ClientID;
        int newId = clsPlans_BLL.AddPlanWithDetails(newPlan);
        newPlan.PlanID = newId;
        return CreatedAtRoute("GetPlanWithDetailsById", new { PlanID = newId }, newPlan);
    }

    // Update an existing Plan with related data
    [HttpPut("Plan/{PlanID}", Name = "UpdatePlanWithDetails")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PlanResponseDTO> UpdatePlanWithDetails(int PlanID, [FromBody] PlanResponseDTO updatedPlan)
    {
        if (updatedPlan == null || updatedPlan.Output == null || updatedPlan.Input == null)
            return BadRequest("Plan data, Output, and Input are required.");

        var existingPlan = clsPlans_BLL.GetPlanWithDetailsById(PlanID);
        if (existingPlan == null)
            return NotFound($"Plan with ID {PlanID} not found.");

        updatedPlan.PlanID = PlanID;
        bool isUpdated = clsPlans_BLL.UpdatePlanWithDetails(updatedPlan, PlanID);
        return isUpdated ? Ok(updatedPlan) : BadRequest("Failed to update plan.");
    }

    // Delete a Plan and its related data
    [HttpDelete("Plan/{PlanID}", Name = "DeletePlanWithDetails")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeletePlanWithDetails(int PlanID)
    {
        bool isDeleted = clsPlans_BLL.DeletePlanWithDetails(PlanID);
        return isDeleted
            ? Ok($"Plan with ID {PlanID} deleted successfully.")
            : NotFound($"Plan with ID {PlanID} not found.");
    }

    // GET: api/Plans/ByClient/{ClientID}
    [HttpGet("Plan/{ClientID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<PlanResponseDTO>> GetPlansWithDetailsByClientId(int ClientID)
    {
        var plans = clsPlans_BLL.GetPlansWithDetailsByClientId(ClientID);

        if (plans == null || plans.Count == 0)
        {
            return NotFound($"No plans found for ClientID: {ClientID}");
        }

        return Ok(plans);
    }

    // GET: api/Plants
    [HttpGet("AllPlants", Name = "GetAllPlantsWithDetails")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<string>> GetAllPlants()
    {
        var plants = clsPlants_BLL.GetAllPlantNames();

        if (plants == null || plants.Count == 0)
        {
            return NotFound("No plants found.");
        }

        return Ok(plants);
    }

    // GET: api/Plants/5
    //[HttpGet("Plant/{PlantID}")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlantDTO))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public ActionResult<PlantDTO> GetPlantById(int PlantID)
    //{
    //    var plant = clsPlants_BLL.GetPlantById(PlantID);

    //    if (plant == null)
    //    {
    //        return NotFound($"Plant with ID {PlantID} not found.");
    //    }

    //    return Ok(plant);
    //}

    // GET: api/Materials
    [HttpGet("AllMaterials")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<string>> GetAllMaterials()
    {
        var materials = clsMaterials_BLL.GetAllMaterialNames();

        if (materials == null || materials.Count == 0)
        {
            return NotFound("No materials found.");
        }

        return Ok(materials);
    }

    //// GET: api/Materials/5
    //[HttpGet("Material/{MaterialsID}")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MaterialDTO))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public ActionResult<SuggestedMaterialResponseDTO> GetMaterialById(int MaterialsID)
    //{
    //    var material = clsSuggestedMaterials_BLL.GetSuggestedMaterialNameById(MaterialsID);

    //    if (material == null)
    //    {
    //        return NotFound($"Material with ID {MaterialsID} not found.");
    //    }

    //    return Ok(material);
    //}

    // GET: api/FarmingTools
    [HttpGet("AllFarmingTools")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<string>> GetAllFarmingTools()
    {
        var farmingTools = clsFarmingTools_BLL.GetAllFarmingToolNames();

        if (farmingTools == null || farmingTools.Count == 0)
        {
            return NotFound("No farming tools found.");
        }

        return Ok(farmingTools);
    }

    // GET: api/FarmingTools/5
    //[HttpGet("FarmingTool/{FarmingToolID}")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FarmingToolDTO))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public ActionResult<FarmingToolDTO> GetFarmingToolById(int FarmingToolID)
    //{
    //    var farmingTool = clsFarmingTools_BLL.GetFarmingToolById(FarmingToolID);

    //    if (farmingTool == null)
    //    {
    //        return NotFound($"Farming tool with ID {FarmingToolID} not found.");
    //    }

    //    return Ok(farmingTool);
    //}

    // GET: api/IrrigationSystems
    [HttpGet("AllIrrigationSystems")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<string>> GetAllIrrigationSystems()
    {
        var irrigationSystems = clsIrrigationSystems_BLL.GetAllIrrigationSystemNames();

        if (irrigationSystems == null || irrigationSystems.Count == 0)
        {
            return NotFound("No irrigation systems found.");
        }

        return Ok(irrigationSystems);
    }

    // GET: api/IrrigationSystems/5
    //[HttpGet("IrrigationSystem/{IrrigationSystemID}")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IrrigationSystemDTO))]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public ActionResult<IrrigationSystemDTO> GetIrrigationSystemById(int IrrigationSystemID)
    //{
    //    var irrigationSystem = clsIrrigationSystems_BLL.GetIrrigationSystemById(IrrigationSystemID);

    //    if (irrigationSystem == null)
    //    {
    //        return NotFound($"Irrigation system with ID {IrrigationSystemID} not found.");
    //    }

    //    return Ok(irrigationSystem);
    //}

    // GET: api/Notifications/AllClientNotifications
    [HttpGet("Notifications/AllClientNotifications")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotificationDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<NotificationDTO>> GetAllClientNotifications()
    {
        var notifications = clsClientNotifications_BLL.GetAllClientNotifications();

        if (notifications == null || notifications.Count == 0)
        {
            return NotFound("No client notifications found.");
        }

        return Ok(notifications);
    }

    // GET: api/AllNotifications
    [HttpGet("AllNotifications")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotificationDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<NotificationDTO>> GetAllNotifications()
    {
        var notifications = clsNotifications_BLL.GetAllNotifications();

        if (notifications == null || notifications.Count == 0)
        {
            return NotFound("No notifications found.");
        }

        return Ok(notifications);
    }

    // GET: api/Notifications/5
    [HttpGet("Notification/{NotificationID}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<NotificationDTO> GetNotificationById(int NotificationID)
    {
        var notification = clsNotifications_BLL.GetNotificationById(NotificationID);

        if (notification == null)
        {
            return NotFound($"Notification with ID {NotificationID} not found.");
        }

        return Ok(notification);
    }

    // GET: api/Notifications/5
    [HttpGet("Notification/ClientID={ClientID}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<NotificationDTO>> GetNotificationByClientId(int ClientID)
    {
        var notification = clsClientNotifications_BLL.GetClientNotificationByClientId(ClientID);

        if (notification == null)
        {
            return NotFound($"Notification with ID {ClientID} not found.");
        }

        return Ok(notification);
    }

    // DELETE: api/Notifications/5
    [HttpDelete("Notification/{NotificationID}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteNotification(int NotificationID)
    {
        if (NotificationID <= 0)
        {
            return BadRequest("Invalid Notification ID.");
        }

        try
        {
            bool isDeleted = clsNotifications_BLL.DeleteNotification(NotificationID);

            if (!isDeleted)
            {
                return NotFound($"Notification with ID {NotificationID} not found.");
            }

            return NoContent(); // 204 No Content
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class AiPromptRequest
    {
        public string Prompt { get; set; }
    }

    [HttpPost("GeneratePlan")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OutputResponseDTO>> GeneratePlan([FromBody] InputResponseDTO inputResponse)
    {
        GeminiService _geminiService = new GeminiService();

        try
        {
            // Send input to AI
            var aiResponse = await _geminiService.GenerateNormalPlan(inputResponse);

            if (aiResponse != null)
                return Ok(aiResponse);
            else
                return BadRequest("Try again later");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("Ai")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> AskAi([FromBody] AiPromptRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest("Prompt is required.");
        }
        GeminiService geminiService = new GeminiService();
        try
        {
            var response = await geminiService.TalkToAi(request.Prompt);
            return Ok(new { Res = response });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("Tip")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetAgricultureTip()
    {
        GeminiService geminiService = new();
        string tip = await geminiService.GetAgricultureTip();
        return Ok(new { Res = tip });
    }

    [HttpGet("Suggestion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetAgricultureSuggestion()
    {
        GeminiService geminiService = new();
        var Suggestion = await geminiService.GetAgricultureSuggestion();
        return Ok(new { Res = Suggestion });
    }

    /// Retrieve all plan summaries.
    [HttpGet("Plans/summaries")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Success response
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response
    public ActionResult<List<PlanSummaryDTO>> GetAllPlanSummaries()
    {
        var planSummaries = clsPlans_BLL.GetAllPlanSummaries();

        if (planSummaries.Count == 0)
        {
            return NotFound("No plans found to get summaries.");
        }

        return Ok(planSummaries);
    }

    /// Mark a plan as completed.
    [HttpPut("Plan/SetAsCompleted/{planID}")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Success response
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response
    public IActionResult UpdatePlanCompleted(int planID)
    {
        bool isUpdated = clsPlans_BLL.UpdatePlanCompleted(planID);

        if (!isUpdated)
        {
            return NotFound("Plan not found.");
        }

        return Ok("Plan has been completed.");
    }

    [HttpGet("Plans/OrderByArea")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Success response
    public ActionResult<List<PlanAreaDetailsDTO>> GetPlanAreaDetailsOrderedByArea()
    {
        var planAreaDetails = clsPlanPrizes_BLL.GetPlanAreaDetailsOrderedByArea();

        if (planAreaDetails.Count == 0)
            return NotFound("No completed plans yet.");

        return Ok(planAreaDetails);
    }

    [HttpGet("Plan/ProduceWinner")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Success response
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Not found response
    public IActionResult GetPlanWinner()
    {
        var planWinner = clsPlanPrizes_BLL.GetPlanWinner();

        if (planWinner == null)
        {
            return NotFound("No plan winner found.");
        }

        return Ok(planWinner);
    }

    [HttpGet("Prizes/Coming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<List<PrizeDTO>> GetAllComingPrizes()
    {
        try
        {
            var prizes = clsPrizes_BLL.GetAllComingPrizes();
            if (prizes.Count == 0)
            {
                return NotFound("No coming prizes.");
            }
            return Ok(prizes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("Prizes/Nearest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PrizeDTO> GetNearestPrize()
    {
        try
        {
            var prize = clsPrizes_BLL.GetNearestPrize();

            if (prize == null)
                return NotFound("No upcoming prize found.");

            return Ok(prize);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("Prizes/{PrizeID}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PrizeDTO> GetPrizeById(int PrizeID)
    {
        try
        {
            var prize = clsPrizes_BLL.GetPrizeById(PrizeID);

            if (prize == null)
                return NotFound($"Prize with ID {PrizeID} not found.");

            return Ok(prize);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Add a new prize.
    /// </summary>
    /// <param name="dto">The prize data to add.</param>
    /// <returns>The newly created prize's ID.</returns>
    [HttpPost("Prizes/Add")]
    [ProducesResponseType(StatusCodes.Status201Created)] // Created response
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid input response
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Conflict response (prize already exists for the date)
    public IActionResult AddPrize([FromBody] PrizeDTO dto)
    {
        if (dto.Date < DateTime.Today.Date && dto.PrizeMoney <= 0)
        {
            return BadRequest("Data is not valid.");
        }

        try
        {
            int prizeId = clsPrizes_BLL.AddPrize(dto);
            if (prizeId < 0)
                return BadRequest("This date already schedualed.");

            return CreatedAtAction(nameof(GetPrizeById), new { PrizeID = prizeId }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest("This date already schedualed.");
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    [HttpPost("Support")]
    [ProducesResponseType(StatusCodes.Status201Created)] // Created response
    [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid input response
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Unexpected error response
    public ActionResult<SupportDTO> AddSupport([FromBody] PlanSupportDTO dto)
    {
        if ((dto.Price == null && dto.FarmingTool == null) || dto.PlanID <= 0)
        {
            return BadRequest("Data is not valid.");
        }

        try
        {
            // Call the BLL method to add the support
            int supportId = clsSupports_BLL.AddSupport(dto);

            // Return the newly created support's ID
            return Ok("Added.");
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}