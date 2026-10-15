
"use strict";

// ============================================================
// MATH LEARNING PATH — CSE310 MODULE 3
// Grades 3–6 | ASP.NET Core + MySQL
// ============================================================

// The four mathematical skills in the Chilean curriculum.
const MATHEMATICAL_SKILLS = [
    "Representar",
    "Modelar",
    "Resolver problemas",
    "Argumentar y comunicar"
];

const byId = (id) => document.getElementById(id);

function setText(id, value) {
    const element = byId(id);
    if (element) {
        element.textContent = value ?? "";
    }
}

function showElement(id, visible) {
    const element = byId(id);
    if (element) {
        element.hidden = !visible;
    }
}

function showFeedback(id, message, correct = null) {
    const element = byId(id);
    if (!element) return;

    element.textContent = message;

    if (correct === true) {
        element.style.color = "#16803d";
    } else if (correct === false) {
        element.style.color = "#b45309";
    } else {
        element.style.color = "";
    }
}

async function postJson(url, body) {
    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
    });

    const result = await response.json();

    if (!response.ok) {
        throw new Error(
            result.message ||
            result.detail ||
            "Server request failed."
        );
    }

    return result;
}

// ============================================================
// 1. CURRICULUM STATE
// ============================================================

const gradeSelect = byId("grade-select");
const oaSelect = byId("oa-select");
const detailSelect = byId("detail-select");
const skillSelect = byId("skill-select");
const difficultySelect = byId("difficulty-select");

const curriculumGenerateButton =
    byId("generate-curriculum-activity");

const curriculumCheckButton =
    byId("check-curriculum-answer");

const interactiveContainer =
    byId("grade4-interactive-container");

let curriculumRules = [];

let currentCurriculumActivityId = null;
let currentCurriculumResponseType = null;
let currentCurriculumGrade = null;

let currentInteractiveSelection = new Set();
let currentInteractiveParts = 0;

// Prevent outdated requests from replacing a newer activity.
let selectionVersion = 0;

// ============================================================
// 2. GRADE HELPERS
// ============================================================

function getGradeNumber(grade) {
    const text = String(grade ?? "").trim();
    const match = text.match(/^\d+/);

    if (match) {
        return Number(match[0]);
    }

    const names = {
        "tercero básico": 3,
        "cuarto básico": 4,
        "quinto básico": 5,
        "sexto básico": 6
    };

    return names[text.toLowerCase()] ?? null;
}

function sameGrade(a, b) {
    const numberA = getGradeNumber(a);
    const numberB = getGradeNumber(b);

    if (numberA !== null && numberB !== null) {
        return numberA === numberB;
    }

    return String(a) === String(b);
}

// ============================================================
// 3. SELECT HELPERS
// ============================================================

function populateSelect(
    select,
    items,
    placeholder,
    getValue,
    getLabel
) {
    if (!select) return;

    select.replaceChildren();

    const placeholderOption =
        document.createElement("option");

    placeholderOption.value = "";
    placeholderOption.textContent = placeholder;

    select.appendChild(placeholderOption);

    for (const item of items) {
        const option = document.createElement("option");

        option.value = String(getValue(item));
        option.textContent = String(getLabel(item));

        select.appendChild(option);
    }

    select.value = "";
    select.disabled = items.length === 0;
}

function resetSkills() {
    populateSelect(
        skillSelect,
        [],
        "Select mathematical skill",
        (value) => value,
        (value) => value
    );

    updateGenerateButton();
}

function resetDetails() {
    populateSelect(
        detailSelect,
        [],
        "Select an OA desagregado",
        (value) => value,
        (value) => value
    );

    resetSkills();
}

function resetOA() {
    populateSelect(
        oaSelect,
        [],
        "Select an OA",
        (value) => value,
        (value) => value
    );

    resetDetails();
}

// ============================================================
// 4. SELECTED CURRICULUM RULE
// ============================================================

function matchingRules() {
    return curriculumRules.filter((rule) =>
        sameGrade(rule.grade, gradeSelect?.value) &&
        String(rule.oaId) === oaSelect?.value &&
        String(rule.learningDetailId) === detailSelect?.value
    );
}

// The frontend allows any of the four skills.
// The backend still needs to implement skill-specific generation.
function selectedRule() {
    return matchingRules()[0] ?? null;
}

function updateGenerateButton() {
    if (!curriculumGenerateButton) return;

    curriculumGenerateButton.disabled =
        !gradeSelect?.value ||
        !oaSelect?.value ||
        !detailSelect?.value ||
        !skillSelect?.value ||
        !selectedRule();
}

// ============================================================
// 5. STUDENT ACTIVITY INTERFACE
// ============================================================

function configureAnswerInterface(responseType) {
    const isMixed = responseType === "MixedNumber";

    const isInteractive =
        responseType === "FractionBarSelection";

    showElement(
        "regular-answer-container",
        !isMixed && !isInteractive
    );

    showElement(
        "mixed-number-container",
        isMixed
    );

    if (interactiveContainer) {
        interactiveContainer.hidden = !isInteractive;
        interactiveContainer.replaceChildren();
    }

    for (const id of [
        "activity-answer",
        "mixed-whole",
        "mixed-numerator",
        "mixed-denominator"
    ]) {
        const input = byId(id);

        if (input) {
            input.value = "";
        }
    }
}

function clearCurriculumActivity() {
    currentCurriculumActivityId = null;
    currentCurriculumResponseType = null;
    currentCurriculumGrade = null;

    currentInteractiveSelection.clear();
    currentInteractiveParts = 0;

    showElement("curriculum-activity", false);

    setText("activity-title", "");
    setText("activity-question", "");

    showFeedback("activity-feedback", "");

    configureAnswerInterface(null);

    if (curriculumCheckButton) {
        curriculumCheckButton.disabled = true;
    }
}

function handleSelectionChange() {
    selectionVersion++;

    clearCurriculumActivity();

    showFeedback("curriculum-feedback", "");
}

// ============================================================
// 6. LOAD CURRICULUM FROM MYSQL
// ============================================================

async function loadCurriculum() {
    try {
        setText(
            "curriculum-status",
            "Loading curriculum from MySQL..."
        );

        const response = await fetch(
            "/api/fractions/curriculum"
        );

        if (!response.ok) {
            throw new Error(
                "Unable to load curriculum from the server."
            );
        }

        const data = await response.json();

        if (!Array.isArray(data.curriculum)) {
            throw new Error(
                "Invalid curriculum response."
            );
        }

        curriculumRules = data.curriculum;

        const grades = [
            ...new Set(
                curriculumRules.map((rule) =>
                    String(rule.grade)
                )
            )
        ];

        grades.sort((a, b) =>
            (getGradeNumber(a) ?? 99) -
            (getGradeNumber(b) ?? 99)
        );

        populateSelect(
            gradeSelect,
            grades,
            "Select a grade",
            (grade) => grade,
            (grade) => grade
        );

        setText(
            "curriculum-status",
            `${curriculumRules.length} curriculum rules loaded from MySQL.`
        );

        resetOA();

    } catch (error) {
        setText("curriculum-status", error.message);

        console.error("Curriculum error:", error);
    }
}

// ============================================================
// 7. GRADE → LEARNING OBJECTIVE
// ============================================================

gradeSelect?.addEventListener("change", () => {
    handleSelectionChange();
    resetOA();

    const rules = curriculumRules.filter((rule) =>
        sameGrade(rule.grade, gradeSelect.value)
    );

    const uniqueObjectives = new Map();

    for (const rule of rules) {
        uniqueObjectives.set(
            String(rule.oaId),
            rule
        );
    }

    const objectives = [
        ...uniqueObjectives.values()
    ];

    objectives.sort((a, b) =>
        String(a.oaCode).localeCompare(
            String(b.oaCode),
            undefined,
            { numeric: true }
        )
    );

    populateSelect(
        oaSelect,
        objectives,
        "Select an OA",
        (rule) => rule.oaId,
        (rule) =>
            `${rule.oaCode} - ${rule.oaDescription}`
    );

    updateGenerateButton();
});

// ============================================================
// 8. OA → OA DESAGREGADO
// ============================================================

oaSelect?.addEventListener("change", () => {
    handleSelectionChange();
    resetDetails();

    const rules = curriculumRules.filter((rule) =>
        sameGrade(rule.grade, gradeSelect.value) &&
        String(rule.oaId) === oaSelect.value
    );

    const uniqueDetails = new Map();

    for (const rule of rules) {
        uniqueDetails.set(
            String(rule.learningDetailId),
            rule
        );
    }

    const details = [
        ...uniqueDetails.values()
    ];

    details.sort((a, b) =>
        Number(a.learningDetailId) -
        Number(b.learningDetailId)
    );

    populateSelect(
        detailSelect,
        details,
        "Select an OA desagregado",
        (rule) => rule.learningDetailId,
        (rule) => rule.learningDetail
    );

    updateGenerateButton();
});

// ============================================================
// 9. OA DESAGREGADO → ALL FOUR MATHEMATICAL SKILLS
// ============================================================

detailSelect?.addEventListener("change", () => {
    handleSelectionChange();
    resetSkills();

    const rules = matchingRules();

    if (rules.length === 0) {
        return;
    }

    // Always display all four mathematical skills.
    populateSelect(
        skillSelect,
        MATHEMATICAL_SKILLS,
        "Select mathematical skill",
        (skill) => skill,
        (skill) => skill
    );

    updateGenerateButton();
});

skillSelect?.addEventListener("change", () => {
    handleSelectionChange();
    updateGenerateButton();
});

difficultySelect?.addEventListener(
    "change",
    handleSelectionChange
);

// ============================================================
// 10. INTERACTIVE FRACTION BAR
// ============================================================

// Backend contract:
// responseType: "FractionBarSelection"
// denominator: integer from 2 to 24
//
// The correct answer remains on the C# server.

function renderFractionBar(activity) {
    if (!interactiveContainer) {
        throw new Error(
            "Interactive container is missing from index.html."
        );
    }

    const parts = Number(activity.denominator);

    if (
        !Number.isInteger(parts) ||
        parts < 2 ||
        parts > 24
    ) {
        throw new Error(
            "The server did not provide a valid fraction-bar denominator."
        );
    }

    currentInteractiveParts = parts;
    currentInteractiveSelection.clear();

    interactiveContainer.replaceChildren();
    interactiveContainer.hidden = false;

    const instructions =
        document.createElement("p");

    instructions.className =
        "interactive-instructions";

    instructions.textContent =
        "Click the parts of the bar to shade them.";

    const bar = document.createElement("div");
    bar.className = "interactive-fraction-bar";

    const progress = document.createElement("p");
    progress.className = "interactive-progress";

    function updateProgress() {
        progress.textContent =
            `${currentInteractiveSelection.size} of ${parts} parts selected`;
    }

    for (let index = 0; index < parts; index++) {
        const part = document.createElement("button");

        part.type = "button";
        part.className = "interactive-fraction-part";

        part.setAttribute(
            "aria-label",
            `Part ${index + 1} of ${parts}`
        );

        part.setAttribute(
            "aria-pressed",
            "false"
        );

        part.addEventListener("click", () => {
            if (currentInteractiveSelection.has(index)) {
                currentInteractiveSelection.delete(index);
            } else {
                currentInteractiveSelection.add(index);
            }

            const selected =
                currentInteractiveSelection.has(index);

            part.classList.toggle(
                "selected",
                selected
            );

            part.setAttribute(
                "aria-pressed",
                String(selected)
            );

            updateProgress();
        });

        bar.appendChild(part);
    }

    const resetButton =
        document.createElement("button");

    resetButton.type = "button";
    resetButton.className = "interactive-reset";
    resetButton.textContent = "Clear Selection";

    resetButton.addEventListener("click", () => {
        currentInteractiveSelection.clear();

        for (const part of bar.children) {
            part.classList.remove("selected");

            part.setAttribute(
                "aria-pressed",
                "false"
            );
        }

        updateProgress();
    });

    interactiveContainer.append(
        instructions,
        bar,
        progress,
        resetButton
    );

    updateProgress();
}

// ============================================================
// 11. MIXED NUMBER ANSWER
// ============================================================

function getMixedNumberAnswer() {
    const wholeText =
        byId("mixed-whole")?.value.trim() ?? "";

    const numeratorText =
        byId("mixed-numerator")?.value.trim() ?? "";

    const denominatorText =
        byId("mixed-denominator")?.value.trim() ?? "";

    if (
        wholeText === "" ||
        numeratorText === "" ||
        denominatorText === ""
    ) {
        throw new Error(
            "Complete the whole number, numerator, and denominator."
        );
    }

    const whole = Number(wholeText);
    const numerator = Number(numeratorText);
    const denominator = Number(denominatorText);

    if (
        !Number.isSafeInteger(whole) ||
        !Number.isSafeInteger(numerator) ||
        !Number.isSafeInteger(denominator) ||
        whole < 0 ||
        numerator < 0 ||
        denominator <= 0
    ) {
        throw new Error(
            "Enter valid whole numbers and a positive denominator."
        );
    }

    return `${whole} ${numerator}/${denominator}`;
}

// ============================================================
// 12. GENERATE STUDENT ACTIVITY
// ============================================================

curriculumGenerateButton?.addEventListener(
    "click",
    async () => {
        handleSelectionChange();

        const rule = selectedRule();

        if (!rule) {
            showFeedback(
                "curriculum-feedback",
                "Select a valid grade, OA, learning detail, and mathematical skill.",
                false
            );
            return;
        }

        const requestVersion = selectionVersion;

        curriculumGenerateButton.disabled = true;

        showFeedback(
            "curriculum-feedback",
            "Generating activity..."
        );

        try {
            const result = await postJson(
                "/api/fractions/generate-by-detail",
                {
                    learningDetailId:
                        Number(rule.learningDetailId),

                    mathematicalSkill:
                        skillSelect.value,

                    difficulty:
                        difficultySelect?.value || "Medium"
                }
            );

            if (requestVersion !== selectionVersion) {
                return;
            }

            if (!result.activityId || !result.question) {
                throw new Error(
                    "The server returned an invalid activity."
                );
            }

            currentCurriculumActivityId =
                result.activityId;

            currentCurriculumResponseType =
                result.responseType || "Text";

            currentCurriculumGrade =
                result.grade;

            setText(
                "activity-title",
                `${result.grade} — ${result.oa}`
            );

            setText(
                "activity-question",
                result.question
            );

            configureAnswerInterface(
                currentCurriculumResponseType
            );

            if (
                currentCurriculumResponseType ===
                "FractionBarSelection"
            ) {
                renderFractionBar(result);
            }

            showElement("curriculum-activity", true);

            if (curriculumCheckButton) {
                curriculumCheckButton.disabled = false;
            }

            showFeedback(
                "curriculum-feedback",
                `${result.activityType} — ${result.difficulty}`
            );

            showFeedback(
                "activity-feedback",
                "Complete the exercise and check your answer."
            );

        } catch (error) {
            if (requestVersion !== selectionVersion) {
                return;
            }

            clearCurriculumActivity();

            showFeedback(
                "curriculum-feedback",
                error.message,
                false
            );

            console.error("Generation error:", error);

        } finally {
            updateGenerateButton();
        }
    }
);

// ============================================================
// 13. CHECK STUDENT ANSWER
// ============================================================

curriculumCheckButton?.addEventListener(
    "click",
    async () => {
        if (!currentCurriculumActivityId) {
            showFeedback(
                "activity-feedback",
                "Generate an activity first.",
                false
            );
            return;
        }

        let studentAnswer = "";

        try {
            if (
                currentCurriculumResponseType ===
                "MixedNumber"
            ) {
                studentAnswer = getMixedNumberAnswer();

            } else if (
                currentCurriculumResponseType ===
                "FractionBarSelection"
            ) {
                if (currentInteractiveParts === 0) {
                    throw new Error(
                        "The fraction bar is not available."
                    );
                }

                studentAnswer =
                    `${currentInteractiveSelection.size}/${currentInteractiveParts}`;

            } else {
                studentAnswer =
                    byId("activity-answer")?.value.trim() || "";
            }

            if (!studentAnswer) {
                throw new Error(
                    "Please enter your answer."
                );
            }

        } catch (error) {
            showFeedback(
                "activity-feedback",
                error.message,
                false
            );
            return;
        }

        const checkedActivityId =
            currentCurriculumActivityId;

        const requestVersion = selectionVersion;

        curriculumCheckButton.disabled = true;

        showFeedback(
            "activity-feedback",
            "Checking answer..."
        );

        try {
            const result = await postJson(
                "/api/fractions/check-generated",
                {
                    activityId: checkedActivityId,
                    studentAnswer
                }
            );

            if (requestVersion !== selectionVersion) {
                return;
            }

            showFeedback(
                "activity-feedback",
                result.message,
                result.correct
            );

        } catch (error) {
            if (requestVersion !== selectionVersion) {
                return;
            }

            showFeedback(
                "activity-feedback",
                error.message,
                false
            );

        } finally {
            if (
                requestVersion === selectionVersion &&
                curriculumCheckButton
            ) {
                curriculumCheckButton.disabled = false;
            }
        }
    }
);

// ============================================================
// 14. LEGACY GRADE 3 ACTIVITY
// ============================================================

// Preserved for compatibility with the existing API.
// This panel remains hidden in the shared interface.

let currentGrade3ActivityId = null;

const grade3GenerateButton =
    byId("generate-activity");

const grade3CheckButton =
    byId("check-generated-answer");

grade3GenerateButton?.addEventListener(
    "click",
    async () => {
        currentGrade3ActivityId = null;
        grade3GenerateButton.disabled = true;

        if (grade3CheckButton) {
            grade3CheckButton.disabled = true;
        }

        const answerInput = byId("generated-answer");

        if (answerInput) {
            answerInput.value = "";
        }

        setText(
            "generated-question",
            "Generating activity..."
        );

        setText("number-line", "");
        showFeedback("generated-feedback", "");

        const request = {
            grade: "3rd Grade",
            field: "Numbers and Operations",
            content: "Fractions",
            oa: "OA11",
            learningDetail: "Common Fractions",
            assessmentIndicator:
                "Representan fracciones de uso común en escalas graduadas y/o en la recta numérica.",
            mathematicalSkill: "Represent",
            difficulty: "Easy",
            exerciseType: "NumberLine"
        };

        try {
            const activity = await postJson(
                "/api/fractions/generate",
                request
            );

            if (!activity.activityId || !activity.question) {
                throw new Error(
                    "Invalid Grade 3 activity response."
                );
            }

            currentGrade3ActivityId =
                activity.activityId;

            setText(
                "assessment-indicator",
                activity.indicator || ""
            );

            setText(
                "generated-question",
                activity.question
            );

            if (grade3CheckButton) {
                grade3CheckButton.disabled = false;
            }

            showFeedback(
                "generated-feedback",
                "Activity ready. Enter your answer."
            );

        } catch (error) {
            setText(
                "generated-question",
                "Unable to load the activity."
            );

            showFeedback(
                "generated-feedback",
                error.message,
                false
            );

        } finally {
            grade3GenerateButton.disabled = false;
        }
    }
);

grade3CheckButton?.addEventListener(
    "click",
    async () => {
        const studentAnswer =
            byId("generated-answer")?.value.trim() || "";

        if (!currentGrade3ActivityId) {
            showFeedback(
                "generated-feedback",
                "Generate an activity first.",
                false
            );
            return;
        }

        if (!/^\d+\s*\/\s*[1-9]\d*$/.test(studentAnswer)) {
            showFeedback(
                "generated-feedback",
                "Enter a fraction such as 1/2 or 3/4.",
                false
            );
            return;
        }

        grade3CheckButton.disabled = true;

        try {
            const result = await postJson(
                "/api/fractions/check-generated",
                {
                    activityId: currentGrade3ActivityId,
                    studentAnswer
                }
            );

            showFeedback(
                "generated-feedback",
                result.message,
                result.correct
            );

        } catch (error) {
            showFeedback(
                "generated-feedback",
                error.message,
                false
            );

        } finally {
            grade3CheckButton.disabled = false;
        }
    }
);

// ============================================================
// 15. LEGACY GRADE 6 FRACTION MODEL
// ============================================================

const legacyNumerator = 3;
const legacyDenominator = 4;

const legacyModel = byId("fraction-model");

if (legacyModel) {
    legacyModel.replaceChildren();

    for (
        let index = 0;
        index < legacyDenominator;
        index++
    ) {
        const part = document.createElement("div");

        part.className =
            "fraction-part" +
            (index < legacyNumerator ? " shaded" : "");

        legacyModel.appendChild(part);
    }
}

byId("check-answer")?.addEventListener(
    "click",
    async () => {
        const value =
            byId("answer")?.value.trim() ?? "";

        const selectedNumerator = Number(value);

        if (
            value === "" ||
            !Number.isInteger(selectedNumerator) ||
            selectedNumerator < 0 ||
            selectedNumerator > legacyDenominator
        ) {
            showFeedback(
                "feedback",
                `Enter a whole number between 0 and ${legacyDenominator}.`,
                false
            );
            return;
        }

        try {
            const result = await postJson(
                "/api/fractions/check",
                {
                    numerator: legacyNumerator,
                    denominator: legacyDenominator,
                    selectedNumerator
                }
            );

            showFeedback(
                "feedback",
                result.message,
                result.correct
            );

        } catch (error) {
            showFeedback(
                "feedback",
                error.message,
                false
            );
        }
    }
);

// ============================================================
// 16. INITIALIZE APPLICATION
// ============================================================

function initializeApplication() {
    // Use one shared student activity panel.
    showElement("grade3-activity", false);
    showElement("grade6-activity", false);

    clearCurriculumActivity();
    resetOA();

    if (gradeSelect) {
        gradeSelect.disabled = true;
    }

    if (grade3CheckButton) {
        grade3CheckButton.disabled = true;
    }

    updateGenerateButton();
    loadCurriculum();
}

initializeApplication();

